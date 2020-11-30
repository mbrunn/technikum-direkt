using System.Collections.Generic;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.FluentValidation;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.BusinessLogic.Models;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Sql.Exceptions;
using TechnikumDirekt.ServiceAgents.Exceptions;
using TechnikumDirekt.ServiceAgents.Interfaces;
using TechnikumDirekt.ServiceAgents.Models;
using TechnikumDirekt.Services.Mapper;
using DalModels = TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.BusinessLogic.Tests
{
    public class TrackingLogicTests
    {
        private IMapper _mapper;
        private IParcelRepository _parcelRepository;
        private IWarehouseLogic _warehouseLogic;
        private IHopRepository _hopRepository;
        private IGeoEncodingAgent _geoEncodingAgent;
        private ILogisticsPartnerAgent _logisticsPartnerAgent;
        
        private NullLogger<TrackingLogic> _logger;

        private readonly Recipient _recipient1 = new Recipient
        {
            Name = "Michi Mango", Street = "TestStreet 1", PostalCode = "1234", City = "Mistelbach Weltstadt",
            Country = "AT"
        };

        private readonly Recipient _recipient2 = new Recipient
        {
            Name = "Benji Bananas", Street = "Banana Street 2", PostalCode = "4242", City = "Banana City",
            Country = "AT"
        };

        private readonly DalModels.Recipient _dalRecipient1 = new DalModels.Recipient
        {
            Name = "Michi Mango", Street = "TestStreet 1", PostalCode = "1234", City = "Mistelbach Weltstadt",
            Country = "AT"
        };

        private readonly DalModels.Recipient _dalRecipient2 = new DalModels.Recipient
        {
            Name = "Benji Bananas", Street = "Banana Street 2", PostalCode = "4242", City = "Banana City",
            Country = "AT"
        };

        private readonly Address _validAddress1 = new Address()
        {
            Street = "TestStreet 1", PostalCode = "1234", City = "Mistelbach Weltstadt",
            Country = "AT"
        };

        private readonly Address _validAddress2 = new Address()
        {
            Street = "Banana Street 2", PostalCode = "4242", City = "Banana City",
            Country = "AT"
        };

        private readonly Address _inValidAddress = new Address()
        {
            Street = "invalidStreet"
        };

        private readonly Hop _validHop = new Hop()
        {
            Code = "ABC123",
            Description = "description",
            HopType = HopType.Truck,
            LocationCoordinates = new Point(123, 123),
            LocationName = "Truck in BananaCity",
            ProcessingDelayMins = 10
        };
        
        #region sampleWarehouseStructure
        
        private static Transferwarehouse TransferwarehouseTran01 = new Transferwarehouse()
        {
            Code = "Tran01",
            Description = "TransferwarehouseTran01",
            HopType = HopType.TransferWarehouse
        };
        
        private static Truck TruckTruc01 = new Truck()
        {
            Code = "Truc01",
            Description = "TruckTruc01",
            HopType = HopType.Truck
        };
        
        private static Truck TruckTruc02 = new Truck()
        {
            Code = "Truc02",
            Description = "TruckTruc02",
            HopType = HopType.Truck
        };
        
        private static Warehouse WarehouseWare02 = new Warehouse()
        {
            Code = "Ware02",
            Description = "WarehouseWare02",
            HopType = HopType.Warehouse,
            Level = 1,
            NextHops = new List<WarehouseNextHops>()
            {
                new WarehouseNextHops(){Hop = TransferwarehouseTran01, TraveltimeMins = 10},
                new WarehouseNextHops(){Hop = TruckTruc01, TraveltimeMins = 20}
            }
        };
        
        private static Warehouse WarehouseWare03 = new Warehouse()
        {
            Code = "Ware03",
            Description = "WarehouseWare03",
            HopType = HopType.Warehouse,
            Level = 1,            
            NextHops = new List<WarehouseNextHops>()
            {
                new WarehouseNextHops(){Hop = TruckTruc02, TraveltimeMins = 10}
            }
        };
        
        private static Warehouse WarehouseWare01 = new Warehouse()
        {
            Code = "Ware01",
            Description = "WarehouseWare01",
            HopType = HopType.Warehouse,
            Level = 0,
            NextHops = new List<WarehouseNextHops>()
            {
                new WarehouseNextHops(){Hop = WarehouseWare02, TraveltimeMins = 10},
                new WarehouseNextHops(){Hop = WarehouseWare03, TraveltimeMins = 20}
            }
        };
        
        private readonly Point _truck01Point = new Point(42.0, 42.0);
        private readonly Point _truck02Point = new Point(24.0, 24.0);
        private readonly Point _tran01Point = new Point(55.0, 55.0);
        
        #endregion

        private readonly Point _validPoint = new Point(42.5, 42.5);
        private readonly Point _inValidPoint = new Point(66.6, 66.6);

        private const string ValidTrackingNumber = "A123BCD23";
        private const string ValidTrackingNumber2 = "B123BCD56";
        private const string InvalidTrackingNumber = "A123BaD23";
        private const string NotfoundTrackingNumber = "000000000";
        private const string ValidHopCode = "ABCD1234";
        private const string InvalidHopCode = "AbdA2a";
        private const string NotfoundHopCode = "ABCD0000";

        private ITrackingLogic _trackingLogic;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var mockMapperConfig = new MapperConfiguration(c => c.AddProfile(new DalMapperProfile()));
            _mapper = new Mapper(mockMapperConfig);

            DalModels.Hop validHop = new DalModels.Warehouse
            {
                HopType = DalModels.HopType.Warehouse,
                Code = ValidHopCode,
                Description = "Descriptive description",
                ProcessingDelayMins = 100,
                LocationName = "Benjis Hut",
                LocationCoordinates = new Point(43.645074, -115.993081),
                Level = 5,
                NextHops = new List<DalModels.Hop>()
            };
            
            var validParcel = new DalModels.Parcel
            {
                TrackingId = ValidTrackingNumber,
                Weight = 2.0f,
                Sender = _dalRecipient1,
                Recipient = _dalRecipient2,
                HopArrivals = new List<DalModels.HopArrival>
                    {new DalModels.HopArrival {HopCode = ValidHopCode, ParcelTrackingId = ValidTrackingNumber, Hop = validHop}}
            };

            /* ------------- Mock ParcelRepository Setup ------------- */
            var mockParcelRepository = new Mock<IParcelRepository>();
            // Setup - GetByTrackingId
            mockParcelRepository.Setup(m => m.GetByTrackingId(It.IsAny<string>())).Returns(validParcel);
            mockParcelRepository.Setup(m => m.GetByTrackingId(NotfoundTrackingNumber))
                .Throws<DataAccessNotFoundException>();
            // Setup - Update
            mockParcelRepository.Setup(m => m.Update(validParcel));
            // Setup - Add
            mockParcelRepository.Setup(m => m.Add(validParcel)).Returns(ValidTrackingNumber);

            _parcelRepository = mockParcelRepository.Object;

            /* ------------- Mock HopRepository Setup ------------- */
            var mockHopRepository = new Mock<IHopRepository>();
            // Setup - GetHopByCode
            mockHopRepository.Setup(m => m.GetHopByCode(It.IsAny<string>())).Returns(validHop);
            mockHopRepository.Setup(m => m.GetHopByCode(NotfoundHopCode)).Throws<DataAccessNotFoundException>();

            /* ------------- Mock HopRepository Setup ------------- */
            var geoEncodingAgent = new Mock<IGeoEncodingAgent>();
            // Setup - EncodeAddress
            geoEncodingAgent.Setup(m => m.EncodeAddress(It.IsAny<Address>())).Returns(_validPoint);
            geoEncodingAgent.Setup(m => m.EncodeAddress(_inValidAddress)).Throws<ServiceAgentsNotFoundException>();

            /* ------------- Mock HopRepository Setup ------------- */
            var warehouseLogic = new Mock<IWarehouseLogic>();
            // Setup - GetHopContainingPoint
            warehouseLogic.Setup(m => m.GetHopContainingPoint(_validPoint)).Returns(_validHop);
            warehouseLogic.Setup(m => m.GetHopContainingPoint(_inValidPoint)).Returns(new Hop());
            
            warehouseLogic.Setup(m => m.GetHopContainingPoint(It.Is<Point>(point => point == _truck01Point)))
                .Returns(TruckTruc01);
            warehouseLogic.Setup(m => m.GetHopContainingPoint(It.Is<Point>(point => point == _truck02Point)))
                .Returns(TruckTruc02);
            warehouseLogic.Setup(m => m.GetHopContainingPoint(It.Is<Point>(point => point == _tran01Point)))
                .Returns(TransferwarehouseTran01);
            
            _warehouseLogic = warehouseLogic.Object;
            _geoEncodingAgent = geoEncodingAgent.Object;
            _hopRepository = mockHopRepository.Object;
            _logger = NullLogger<TrackingLogic>.Instance;
        }

        [SetUp]
        public void Setup()
        {
            _trackingLogic = new TrackingLogic(new ParcelValidator(), new RecipientValidator(),
                new HopArrivalValidator(), new HopValidator(),
                _hopRepository, _parcelRepository, _geoEncodingAgent,_logisticsPartnerAgent, _warehouseLogic, _mapper, _logger);
        }

        #region ReportParcelDelivery Tests

        [Test]
        public void ReportParcelDelivery_DoesNotThrow_WithValidTrackingId()
        {
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            parcel.TrackingId = _trackingLogic.SubmitParcel(parcel);

            Assert.DoesNotThrow(() => _trackingLogic.ReportParcelDelivery(parcel.TrackingId));
        }

        [Test]
        public void ReportParcelDelivery_Throws_WithNotfoundTrackingId()
        {
            Assert.Throws<BusinessLogicNotFoundException>(() =>
                _trackingLogic.ReportParcelDelivery(NotfoundTrackingNumber));
        }

        [Test]
        public void ReportParcelDelivery_Throws_WithInvalidTrackingId()
        {
            Assert.Throws<BusinessLogicValidationException>(() =>
                _trackingLogic.ReportParcelDelivery(InvalidTrackingNumber));
        }

        #endregion

        #region ReportParcelHop Tests

        [Test]
        public void ReportParcelHop_DoesNotThrow_WithValidTrackingIdAndHopCode()
        {
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            _trackingLogic.SubmitParcel(parcel);

            Assert.DoesNotThrow(() => _trackingLogic.ReportParcelHop(parcel.TrackingId, ValidHopCode));
        }

        [Test]
        public void ReportParcelHop_Throws_WithNotfoundTrackingId()
        {
            Assert.Throws<BusinessLogicNotFoundException>(() =>
                _trackingLogic.ReportParcelHop(NotfoundTrackingNumber, ValidHopCode));
        }

        [Test]
        public void ReportParcelHop_Throws_WithNotfoundHopCode()
        {
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            _trackingLogic.SubmitParcel(parcel);

            Assert.Throws<BusinessLogicNotFoundException>(() =>
                _trackingLogic.ReportParcelHop(parcel.TrackingId, NotfoundHopCode));
        }

        [Test]
        public void ReportParcelHop_Throws_WithInvalidTrackingId()
        {
            Assert.Throws<BusinessLogicValidationException>(
                () => _trackingLogic.ReportParcelHop(InvalidTrackingNumber, ValidHopCode));
        }

        [Test]
        public void ReportParcelHop_Throws_WithInvalidHopCode()
        {
            Assert.Throws<BusinessLogicValidationException>(
                () => _trackingLogic.ReportParcelHop(ValidTrackingNumber, InvalidHopCode));
        }

        #endregion

        #region SubmitParcel Tests
        
        [Test]
        public void SubmitParcel_DoesNotThrow_WithValidParcel()
        {
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            Assert.DoesNotThrow(() => _trackingLogic.SubmitParcel(parcel));
        }

        [Test]
        public void SubmitParcel_Throws_WithInvalidParcel()
        {
            var parcel = new Parcel
            {
                Weight = -2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            Assert.Throws<BusinessLogicValidationException>(() => _trackingLogic.SubmitParcel(parcel));
        }

        #endregion

        #region TrackParcel Tests

        [Test]
        public void TrackParcel_DoesNotThrow_WithValidAndExistingTrackingId()
        {
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>()
                ,
                FutureHops = new List<HopArrival>()
            };

            parcel.TrackingId = _trackingLogic.SubmitParcel(parcel);

            Parcel result = null;
            Assert.DoesNotThrow(() => { result = _trackingLogic.TrackParcel(parcel.TrackingId); });

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.TrackingId);
            // Cannot check for equal tracking id because the parcel is not persisted in any way
            // Assert.AreEqual(result.TrackingId, parcel.TrackingId);
        }

        [Test]
        public void TrackParcel_Throws_WithNonexistentTrackingId()
        {
            Assert.Throws<BusinessLogicNotFoundException>(() => _trackingLogic.TrackParcel(NotfoundTrackingNumber));
        }

        [Test]
        public void TrackParcel_Throws_WithInvalidTrackingId()
        {
            Assert.Throws<BusinessLogicValidationException>(() => _trackingLogic.TrackParcel(InvalidTrackingNumber));
        }

        #endregion

        #region TransitionParcelFromPartner Tests

        [Test]
        public void TransitionParcelFromPartner_DoesNotThrow_WithValidParcelAndTrackingId()
        {
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            Assert.DoesNotThrow(() => _trackingLogic.TransitionParcelFromPartner(parcel, NotfoundTrackingNumber));
        }

        [Test]
        public void TransitionParcelFromPartner_Throws_WithInuseTrackingId()
        {
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            // Repo is mocked -> parcel is not actually persisted
            // Assert.DoesNotThrow(() => _trackingLogic.TransitionParcelFromPartner(parcel, ValidTrackingNumber));
            Assert.Throws<BusinessLogicBadArgumentException>(() =>
                _trackingLogic.TransitionParcelFromPartner(parcel, ValidTrackingNumber));
        }

        [Test]
        public void TransitionParcelFromPartner_Throws_WithInvalidTrackingId()
        {
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            Assert.Throws<BusinessLogicValidationException>(() =>
                _trackingLogic.TransitionParcelFromPartner(parcel, InvalidTrackingNumber));
        }

        [Test]
        public void TransitionParcelFromPartner_Throws_WithInvalidParcel()
        {
            var parcel = new Parcel
            {
                Weight = -2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            Assert.Throws<BusinessLogicValidationException>(() =>
                _trackingLogic.TransitionParcelFromPartner(parcel, ValidTrackingNumber));
        }

        #endregion
    }
}