using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
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
using Assert = NUnit.Framework.Assert;
using DalModels = TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.BusinessLogic.Tests
{
    public class TrackingLogicTests
    {
        private IMapper _mapper;
        private IParcelRepository _parcelRepository;
        private IWarehouseLogic _warehouseLogic;
        private IHopRepository _hopRepository;
        private IWebhookRepository _webhookRepository;
        private IGeoEncodingAgent _geoEncodingAgent;
        private ILogisticsPartnerAgent _logisticsPartnerAgent;
        private ITrackingLogic _trackingLogic;
        private IWebhookServiceAgent _webhookServiceAgent;
        
        private NullLogger<TrackingLogic> _logger;

        private const string ValidRecipientStreetName = "TestStreet 1";
        private readonly Point _truck01Point = new Point(42.0, 42.0);
        private readonly Point _truck02Point = new Point(24.0, 24.0);
        private readonly Point _tran01Point = new Point(55.0, 55.0);
        
        private readonly Point _validPoint = new Point(42.5, 42.5);
        private readonly Point _inValidPoint = new Point(66.6, 66.6);

        private const string ValidTrackingId = "A123BCD23";
        private const string ValidTrackingNumber2 = "B123BCD56";
        private const string InValidTrackingId = "A123BaD23";
        private const string NotfoundTrackingId = "000000000";
        private const string ValidHopCode = "ABCD1234";
        private const string InvalidHopCode = "AbdA2a";
        private const string NotfoundHopCode = "ABCD0000";

        private const long ExistingWebhookId = 1;
        private const long ExistingWebhookId2 = 2;
        private const long NonExistingWebhookId = 99;
        private const string ValidUrl = "http://www.yeetmann-gruppe.at";

        private readonly Recipient _recipient1 = new Recipient
        {
            Name = "Michi Mango", Street = ValidRecipientStreetName, PostalCode = "1234", City = "Mistelbach Weltstadt",
            Country = "AT"
        };

        private readonly Recipient _recipient2 = new Recipient
        {
            Name = "Benji Bananas", Street = ValidRecipientStreetName, PostalCode = "4242", City = "Banana City",
            Country = "AT"
        };

        private readonly DalModels.Recipient _dalRecipient1 = new DalModels.Recipient
        {
            Name = "Michi Mango", Street = ValidRecipientStreetName, PostalCode = "1234", City = "Mistelbach Weltstadt",
            Country = "AT"
        };

        private readonly DalModels.Recipient _dalRecipient2 = new DalModels.Recipient
        {
            Name = "Benji Bananas", Street = ValidRecipientStreetName, PostalCode = "4242", City = "Banana City",
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
            Code = ValidHopCode,
            Description = "description",
            HopType = HopType.Truck,
            LocationCoordinates = new Point(123, 123),
            LocationName = "Truck in BananaCity",
            ProcessingDelayMins = 10
        };
        
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

        private static DalModels.Warehouse WarehouseWare01Dal = new DalModels.Warehouse()
        {
            Code = WarehouseWare01.Code,
            Description = WarehouseWare01.Description,
            HopType = (DalModels.HopType) WarehouseWare01.HopType,
            Level = WarehouseWare01.Level,
            ParentWarehouse = null,
            ParentWarehouseCode = null
        };
        
        private static DalModels.Warehouse WarehouseWare02Dal = new DalModels.Warehouse()
        {
            Code = WarehouseWare02.Code,
            Description = WarehouseWare02.Description,
            HopType = (DalModels.HopType) WarehouseWare02.HopType,
            Level = WarehouseWare02.Level,
            ParentWarehouse = WarehouseWare01Dal,
            ParentWarehouseCode = WarehouseWare01Dal.Code
        };
        
        private static DalModels.Warehouse WarehouseWare03Dal = new DalModels.Warehouse()
        {
            Code = WarehouseWare03.Code,
            Description = WarehouseWare03.Description,
            HopType = (DalModels.HopType) WarehouseWare03.HopType,
            Level = WarehouseWare03.Level,
            ParentWarehouse = WarehouseWare01Dal,
            ParentWarehouseCode = WarehouseWare01Dal.Code
        };
        
        private static DalModels.Truck TruckTruc01Dal = new DalModels.Truck()
        {
            Code = TruckTruc01.Code,
            Description = TruckTruc01.Description,
            HopType = (DalModels.HopType) TruckTruc01.HopType,
            ParentWarehouse = WarehouseWare02Dal,
            ParentWarehouseCode = WarehouseWare02Dal.Code
        };
        
        private static DalModels.Truck TruckTruc02Dal = new DalModels.Truck()
        {
            Code = WarehouseWare03.Code,
            Description = WarehouseWare03.Description,
            HopType = (DalModels.HopType) WarehouseWare03.HopType,
            ParentWarehouse = WarehouseWare03Dal,
            ParentWarehouseCode = WarehouseWare03Dal.Code
        };
        
        private static DalModels.Transferwarehouse TransferwarehouseTran01Dal = new DalModels.Transferwarehouse()
        {
            Code = TransferwarehouseTran01.Code,
            Description = TransferwarehouseTran01.Description,
            HopType = (DalModels.HopType) TransferwarehouseTran01.HopType,
            ParentWarehouse = WarehouseWare02Dal,
            ParentWarehouseCode = WarehouseWare02Dal.Code
        };
        
        private readonly Recipient truck01Recipient = new Recipient()
        {
            City = "BananaCity",
            Country = "Austria",
            PostalCode = "A-1000",
            Street = "TruckStreet 01"
        };
        
        private readonly Recipient truck02Recipient = new Recipient()
        {
            City = "BananaCity",
            Country = "Austria",
            PostalCode = "A-1000",
            Street = "TruckStreet 02"
        };
        
        private readonly Recipient trans01Recipient = new Recipient()
        {
            City = "BananaCity",
            Country = "Austria",
            PostalCode = "A-1000",
            Street = "TransferWarehouseStreet 01"
        };
        
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
                TrackingId = ValidTrackingId,
                Weight = 2.0f,
                Sender = _dalRecipient1,
                Recipient = _dalRecipient2,
                HopArrivals = new List<DalModels.HopArrival>
                    {new DalModels.HopArrival {HopCode = ValidHopCode, ParcelTrackingId = ValidTrackingId, Hop = validHop}}
            };

            var validWebhook1 = new DalModels.Webhook()
            {
                CreationDate = DateTime.Now,
                Id = ExistingWebhookId,
                Parcel = validParcel,
                Url = ValidUrl
            };
            
            var validWebhook2 = new DalModels.Webhook()
            {
                CreationDate = DateTime.Now,
                Id = ExistingWebhookId2,
                Parcel = validParcel,
                Url = ValidUrl
            };
            
            var dalWebhooks = new List<DalModels.Webhook>()
            {
                validWebhook1,
                validWebhook2
            };
            
            /* ------------- Mock ParcelRepository Setup ------------- */
            var mockParcelRepository = new Mock<IParcelRepository>();
            // Setup - GetByTrackingId
            mockParcelRepository.Setup(m => m.GetByTrackingId(It.IsAny<string>())).Returns(validParcel);
            mockParcelRepository.Setup(m => m.GetByTrackingId(NotfoundTrackingId))
                .Throws<DataAccessNotFoundException>();
            // Setup - Update
            mockParcelRepository.Setup(m => m.Update(validParcel));
            // Setup - Add
            mockParcelRepository.Setup(m => m.Add(validParcel)).Returns(ValidTrackingId);

            _parcelRepository = mockParcelRepository.Object;

            /* ------------- Mock HopRepository Setup ------------- */
            var mockHopRepository = new Mock<IHopRepository>();
            // Setup - GetHopByCode
            mockHopRepository.Setup(m => m.GetHopByCode(It.Is<string>(hopCode => hopCode == TruckTruc01.Code))).Returns(TruckTruc01Dal);
            mockHopRepository.Setup(m => m.GetHopByCode(It.Is<string>(hopCode => hopCode == TruckTruc02.Code))).Returns(TruckTruc02Dal);
            mockHopRepository.Setup(m => m.GetHopByCode(It.Is<string>(hopCode => hopCode == TransferwarehouseTran01.Code))).Returns(TransferwarehouseTran01Dal);
            mockHopRepository.Setup(m => m.GetHopByCode(It.Is<string>(hopCode => hopCode == WarehouseWare01.Code))).Returns(WarehouseWare01Dal);
            mockHopRepository.Setup(m => m.GetHopByCode(It.Is<string>(hopCode => hopCode == WarehouseWare02.Code))).Returns(WarehouseWare02Dal);
            mockHopRepository.Setup(m => m.GetHopByCode(It.Is<string>(hopCode => hopCode == WarehouseWare03.Code))).Returns(WarehouseWare03Dal);
            
            mockHopRepository.Setup(m => m.GetHopByCode(It.Is<string>(hopCode => hopCode == ValidHopCode))).Returns(validHop);
            mockHopRepository.Setup(m => m.GetHopByCode(NotfoundHopCode)).Throws<DataAccessNotFoundException>();

            /* ------------- Mock GeoEncodingAgent Setup ------------- */
            var geoEncodingAgent = new Mock<IGeoEncodingAgent>();
            // Setup - EncodeAddress
            geoEncodingAgent.Setup(m => m.EncodeAddress(It.Is<Address>(address => address.Street == "TruckStreet 01"))).Returns(_truck01Point);
            geoEncodingAgent.Setup(m => m.EncodeAddress(It.Is<Address>(address => address.Street == "TruckStreet 02"))).Returns(_truck02Point);
            geoEncodingAgent.Setup(m => m.EncodeAddress(It.Is<Address>(address => address.Street == "TransferWarehouseStreet 01"))).Returns(_tran01Point);
            
            geoEncodingAgent.Setup(m => m.EncodeAddress(It.Is<Address>(address => address.Street == ValidRecipientStreetName))).Returns(_validPoint);
            geoEncodingAgent.Setup(m => m.EncodeAddress(_inValidAddress)).Throws<ServiceAgentsNotFoundException>();

            /* ------------- Mock WarehouseLogc Setup ------------- */
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
            
            /* ------------- Mock WebhookRepository Setup ------------- */
            var webhookRepository = new Mock<IWebhookRepository>();
            // Setup - GetAllSubscribersByTrackingId
            webhookRepository.Setup(wr => wr.GetAllSubscribersByTrackingId(ValidTrackingId))
                .Returns(dalWebhooks);
            webhookRepository.Setup(wr => wr.GetAllSubscribersByTrackingId(NotfoundTrackingId))
                .Throws<DataAccessNotFoundException>();

            // Setup - AddSubscription
            webhookRepository.Setup(wr =>
                wr.AddSubscription(It.Is<DalModels.Webhook>(wh => wh.Parcel.TrackingId == ValidTrackingId)));
            webhookRepository.Setup(wr => 
                wr.AddSubscription(It.Is<DalModels.Webhook>(wh => wh.Parcel.TrackingId == InValidTrackingId)))
                .Throws<DataAccessNotFoundException>();
            
            // Setup - RemoveSubscription
            webhookRepository.Setup(wr => wr.RemoveSubscription(ExistingWebhookId));
            webhookRepository.Setup(wr => wr.RemoveSubscription(NonExistingWebhookId))
                .Throws<DataAccessNotFoundException>();
            
            /* ------------- Mock WebhookServiceAgent Setup ------------- */
            var webhookServiceAgent = new Mock<IWebhookServiceAgent>();
            // Setup - NotifySubscriber
            webhookServiceAgent.Setup(agent => agent.NotifySubscriber(It.Is<Webhook>(webhook => 
                webhook.Id == ExistingWebhookId)));
            webhookServiceAgent.Setup(agent => agent.NotifySubscriber(It.Is<Webhook>(webhook => 
                webhook.Id == NonExistingWebhookId))).Throws<ServiceAgentsBadResponseException>();
            
            _warehouseLogic = warehouseLogic.Object;
            _geoEncodingAgent = geoEncodingAgent.Object;
            _hopRepository = mockHopRepository.Object;
            _webhookRepository = webhookRepository.Object;
            _webhookServiceAgent = webhookServiceAgent.Object;
            _logger = NullLogger<TrackingLogic>.Instance;
        }

        [SetUp]
        public void Setup()
        {
            _trackingLogic = new TrackingLogic(new ParcelValidator(), new RecipientValidator(),
                new HopArrivalValidator(), new HopValidator(),
                _hopRepository, _parcelRepository, _webhookRepository, _geoEncodingAgent,_logisticsPartnerAgent, _webhookServiceAgent, _warehouseLogic, _mapper, _logger);
        }

        #region ReportParcelDelivery Tests

        [Test]
        public void ReportParcelDelivery_DoesNotThrow_WithValidTrackingId()
        {
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = truck01Recipient,
                Recipient = truck02Recipient,
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
                _trackingLogic.ReportParcelDelivery(NotfoundTrackingId));
        }

        [Test]
        public void ReportParcelDelivery_Throws_WithInvalidTrackingId()
        {
            Assert.Throws<BusinessLogicValidationException>(() =>
                _trackingLogic.ReportParcelDelivery(InValidTrackingId));
        }

        #endregion

        #region ReportParcelHop Tests

        [Test]
        public void ReportParcelHop_DoesNotThrow_WithValidTrackingIdAndHopCode()
        {
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = truck01Recipient,
                Recipient = truck02Recipient,
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
                _trackingLogic.ReportParcelHop(NotfoundTrackingId, ValidHopCode));
        }

        [Test]
        public void ReportParcelHop_Throws_WithNotfoundHopCode()
        {
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = truck01Recipient,
                Recipient = truck02Recipient,
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
                () => _trackingLogic.ReportParcelHop(InValidTrackingId, ValidHopCode));
        }

        [Test]
        public void ReportParcelHop_Throws_WithInvalidHopCode()
        {
            Assert.Throws<BusinessLogicValidationException>(
                () => _trackingLogic.ReportParcelHop(ValidTrackingId, InvalidHopCode));
        }

        #endregion

        #region SubmitParcel Tests
        
        [Test]
        public void SubmitParcel_DoesNotThrow_ValidParcelTruck01ToTruck02()
        {
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = truck01Recipient,
                Recipient = truck02Recipient,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            Assert.DoesNotThrow(() => _trackingLogic.SubmitParcel(parcel));
        }
        
        [Test]
        public void SubmitParcel_DoesNotThrow_ValidParcelTruck02ToTransferWarehouse01()
        {
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = truck02Recipient,
                Recipient = trans01Recipient,
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
                Sender = truck01Recipient,
                Recipient = truck02Recipient,
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
            Assert.Throws<BusinessLogicNotFoundException>(() => _trackingLogic.TrackParcel(NotfoundTrackingId));
        }

        [Test]
        public void TrackParcel_Throws_WithInvalidTrackingId()
        {
            Assert.Throws<BusinessLogicValidationException>(() => _trackingLogic.TrackParcel(InValidTrackingId));
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

            Assert.DoesNotThrow(() => _trackingLogic.TransitionParcelFromPartner(parcel, NotfoundTrackingId));
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
            // Assert.DoesNotThrow(() => _trackingLogic.TransitionParcelFromPartner(parcel, ValidTrackingId));
            Assert.Throws<BusinessLogicBadArgumentException>(() =>
                _trackingLogic.TransitionParcelFromPartner(parcel, ValidTrackingId));
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
                _trackingLogic.TransitionParcelFromPartner(parcel, InValidTrackingId));
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
                _trackingLogic.TransitionParcelFromPartner(parcel, ValidTrackingId));
        }

        #endregion
        
        #region Webhooks

        #region GetAllSubscribersByTrackingId Tests

        [Test]
        public void GetAllSubscribersByTrackingId_DoesNotThrow_WithValidTrackingId()
        {
            //Arrange

            //Act
            var blWebhooks = _trackingLogic.GetAllSubscribersByTrackingId(ValidTrackingId).ToList();
            
            //Assert
            Assert.Greater(blWebhooks.Count(), 1);
            Assert.NotNull(blWebhooks.FirstOrDefault(wh => wh.Id == ExistingWebhookId));
            Assert.NotNull(blWebhooks.FirstOrDefault(wh => wh.Id == ExistingWebhookId2));
        }
        
        [Test]
        public void GetAllSubscribersByTrackingId_ThrowsBLNotFoundException_NotfoundTrackingId()
        {
            //Arrange

            //Act / Assert
            Assert.Throws<BusinessLogicNotFoundException>(() => _trackingLogic.GetAllSubscribersByTrackingId(NotfoundTrackingId));
        }
        
        [Test]
        public void GetAllSubscribersByTrackingId_BLValidationException_InValidTrackingId()
        {
            //Arrange

            //Act / Assert
            Assert.Throws<BusinessLogicValidationException>(() => _trackingLogic.GetAllSubscribersByTrackingId(InValidTrackingId));
        }
        
        #endregion
        
        #region RemoveParcelWebhook Tests

        [Test]
        public void RemoveParcelWebhook_DoesNotThrow_ExistingWebhookId()
        {
            //Arrange

            //Act / Assert
            Assert.DoesNotThrow(() => _trackingLogic.RemoveParcelWebhook(ExistingWebhookId));
        }
        
        [Test]
        public void RemoveParcelWebhook_BLValidationException_InValidTrackingId()
        {
            //Arrange

            //Act / Assert
            Assert.Throws<BusinessLogicNotFoundException>(() => _trackingLogic.RemoveParcelWebhook(NonExistingWebhookId));
        }
        
        #endregion
        
        #region SubscribeParcelWebhook Tests
        
        [Test]
        public void SubscribeParcelWebhook_DoesNotThrow_ValidTrackingId()
        {
            //Arrange

            //Act / Assert
            Assert.DoesNotThrow(() => _trackingLogic.SubscribeParcelWebhook(ValidTrackingId, ValidUrl));
        }
        
        [Test]
        public void SubscribeParcelWebhook_BLNotFoundException_InValidTrackingId()
        {
            //Arrange

            //Act / Assert
            Assert.Throws<BusinessLogicNotFoundException>(() => _trackingLogic.SubscribeParcelWebhook(NotfoundTrackingId, ValidUrl));
        }
        
        [Test]
        public void SubscribeParcelWebhook_BLValidationException_InValidTrackingId()
        {
            //Arrange

            //Act / Assert
            Assert.Throws<BusinessLogicValidationException>(() => _trackingLogic.SubscribeParcelWebhook(InValidTrackingId, ValidUrl));
        }
        
        #endregion

        #endregion
    }
}