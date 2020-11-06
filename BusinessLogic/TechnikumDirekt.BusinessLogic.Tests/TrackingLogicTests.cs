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
using TechnikumDirekt.Services.Mapper;
using DalModels = TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.BusinessLogic.Tests
{
    public class TrackingLogicTests
    {
        private IMapper _mapper;
        private IParcelRepository _parcelRepository;
        private IHopRepository _hopRepository;
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

            var validParcel = new DalModels.Parcel
            {
                TrackingId = ValidTrackingNumber,
                Weight = 2.0f,
                Sender = _dalRecipient1,
                Recipient = _dalRecipient2,
                HopArrivals = new List<DalModels.HopArrival>
                    {new DalModels.HopArrival {HopCode = ValidHopCode, ParcelTrackingId = ValidTrackingNumber}}
            };

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

            /* ------------- Mock ParcelRepository Setup ------------- */
            var mockParcelRepository = new Mock<IParcelRepository>();
            // Setup - GetByTrackingId
            mockParcelRepository.Setup(m => m.GetByTrackingId(It.IsAny<string>())).Returns(validParcel);
            mockParcelRepository.Setup(m => m.GetByTrackingId(NotfoundTrackingNumber)).Returns<DalModels.Parcel>(null);
            // Setup - Update
            mockParcelRepository.Setup(m => m.Update(validParcel));
            // Setup - Add
            mockParcelRepository.Setup(m => m.Add(validParcel)).Returns(ValidTrackingNumber);

            _parcelRepository = mockParcelRepository.Object;

            /* ------------- Mock HopRepository Setup ------------- */
            var mockHopRepository = new Mock<IHopRepository>();
            // Setup - GetHopByCode
            mockHopRepository.Setup(m => m.GetHopByCode(It.IsAny<string>())).Returns(validHop);
            mockHopRepository.Setup(m => m.GetHopByCode(NotfoundHopCode)).Returns<DalModels.Hop>(null);

            _hopRepository = mockHopRepository.Object;
            _logger = NullLogger<TrackingLogic>.Instance;
        }

        [SetUp]
        public void Setup()
        {
            _trackingLogic = new TrackingLogic(new ParcelValidator(), new RecipientValidator(),
                new HopArrivalValidator(), new HopValidator(),
                _hopRepository, _parcelRepository, _mapper, _logger);
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
            Assert.Throws<TrackingLogicException>(() => _trackingLogic.ReportParcelDelivery(NotfoundTrackingNumber));
        }

        [Test]
        public void ReportParcelDelivery_Throws_WithInvalidTrackingId()
        {
            Assert.Throws<ValidationException>(() => _trackingLogic.ReportParcelDelivery(InvalidTrackingNumber));
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
            Assert.Throws<TrackingLogicException>(() =>
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

            Assert.Throws<TrackingLogicException>(() =>
                _trackingLogic.ReportParcelHop(parcel.TrackingId, NotfoundHopCode));
        }

        [Test]
        public void ReportParcelHop_Throws_WithInvalidTrackingId()
        {
            Assert.Throws<ValidationException>(
                () => _trackingLogic.ReportParcelHop(InvalidTrackingNumber, ValidHopCode));
        }

        [Test]
        public void ReportParcelHop_Throws_WithInvalidHopCode()
        {
            Assert.Throws<ValidationException>(
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

            Assert.Throws<ValidationException>(() => _trackingLogic.SubmitParcel(parcel));
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
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            parcel.TrackingId = _trackingLogic.SubmitParcel(parcel);

            Parcel result = null;
            Assert.DoesNotThrow(() => { result = _trackingLogic.TrackParcel(ValidTrackingNumber); });

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.TrackingId);
            // Cannot check for equal tracking id because the parcel is not persisted in any way
            // Assert.AreEqual(result.TrackingId, parcel.TrackingId);
        }

        [Test]
        public void TrackParcel_Throws_WithNonexistentTrackingId()
        {
            Assert.Throws<TrackingLogicException>(() => _trackingLogic.TrackParcel(NotfoundTrackingNumber));
        }

        [Test]
        public void TrackParcel_Throws_WithInvalidTrackingId()
        {
            Assert.Throws<ValidationException>(() => _trackingLogic.TrackParcel(InvalidTrackingNumber));
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
            Assert.Throws<TrackingLogicException>(() =>
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

            Assert.Throws<ValidationException>(() =>
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

            Assert.Throws<ValidationException>(() =>
                _trackingLogic.TransitionParcelFromPartner(parcel, ValidTrackingNumber));
        }

        #endregion
    }
}