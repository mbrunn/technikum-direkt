using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.Services.Controllers;
using TechnikumDirekt.Services.Mapper;
using TechnikumDirekt.Services.Models;
using BlParcel = TechnikumDirekt.BusinessLogic.Models.Parcel;
using BlRecipient = TechnikumDirekt.BusinessLogic.Models.Recipient;

namespace TechnikumDirekt.Services.Tests
{
    [TestFixture]
    public class RecipientApiTests
    {
        private ITrackingLogic _trackingLogic;
        private IMapper _mapper;
        private NullLogger<RecipientApiController> _logger;

        private readonly Recipient _recipient1 = new Recipient
        {
            Name = "Michi Mango",
            Street = "TestStreet 1",
            PostalCode = "1234",
            City = "Mistelbach Weltstadt",
            Country = "AT"
        };

        private readonly Recipient _recipient2 = new Recipient
        {
            Name = "Benji Bananas",
            Street = "Banana Street 2",
            PostalCode = "4242",
            City = "Banana City",
            Country = "AT"
        };

        private const string ValidTrackingNumber = "A123BCD23";
        private const string InvalidTrackingNumber = "A123BaD23";
        private const string NotfoundTrackingNumber = "000000000";

        [OneTimeSetUp]
        public void Setup()
        {
            var mockMapperConfig = new MapperConfiguration(c => c.AddProfile(new BlMapperProfile()));
            _mapper = new AutoMapper.Mapper(mockMapperConfig);
            var validParcel = new BlParcel
            {
                Weight = 2.0f,
                Sender = _mapper.Map<BlRecipient>(_recipient1),
                Recipient = _mapper.Map<BlRecipient>(_recipient2)
            };

            var mockTrackingLogic = new Mock<ITrackingLogic>();
            // Setup - TrackParcel
            mockTrackingLogic.Setup(m => m.TrackParcel(ValidTrackingNumber)).Returns(validParcel);
            mockTrackingLogic.Setup(m => m.TrackParcel(InvalidTrackingNumber)).Throws<BusinessLogicValidationException>();
            mockTrackingLogic.Setup(m => m.TrackParcel(NotfoundTrackingNumber)).Throws<BusinessLogicNotFoundException>();

            _trackingLogic = mockTrackingLogic.Object;
            _logger = NullLogger<RecipientApiController>.Instance;
        }

        [Test]
        public void TrackParcel_ValidParcel_Ok()
        {
            var controller = new RecipientApiController(_trackingLogic, _mapper, _logger);

            var response = controller.TrackParcel(ValidTrackingNumber);

            Assert.IsInstanceOf<OkObjectResult>(response);

            var typedResponse = (OkObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(200, statusCode);
            Assert.IsInstanceOf<TrackingInformation>(typedResponse.Value);
        }

        [Test]
        public void TrackParcel_InvalidParcel_BadRequest()
        {
            var controller = new RecipientApiController(_trackingLogic, _mapper, _logger);

            var response = controller.TrackParcel(InvalidTrackingNumber);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            var typedResponse = (BadRequestObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(400, statusCode);
        }

        [Test]
        public void TrackParcel_NonExistingParcel_NotFound()
        {
            var controller = new RecipientApiController(_trackingLogic, _mapper, _logger);

            var response = controller.TrackParcel(NotfoundTrackingNumber);

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            var typedResponse = (NotFoundObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(404, statusCode);
        }
    }
}