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
    public class SenderApiTests
    {
        private ITrackingLogic _trackingLogic;
        private IMapper _mapper;
        private NullLogger<SenderApiController> _logger;

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
            // Setup - SubmitParcel
            mockTrackingLogic.Setup(m => m.SubmitParcel(validParcel)).Returns(validParcel.TrackingId);
            mockTrackingLogic.Setup(m => m.SubmitParcel(null)).Throws<BusinessLogicValidationException>();

            _trackingLogic = mockTrackingLogic.Object;
            _logger = NullLogger<SenderApiController>.Instance;
        }

        [Test]
        public void SubmitParcel_ValidParcel_Ok()
        {
            var controller = new SenderApiController(_trackingLogic, _mapper, _logger);
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2
            };

            var response = controller.SubmitParcel(parcel);

            Assert.IsInstanceOf<OkObjectResult>(response);

            var typedResponse = (OkObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(200, statusCode);
        }

        [Test]
        public void SubmitParcel_NullParcel_BadRequest()
        {
            var controller = new SenderApiController(_trackingLogic, _mapper, _logger);

            var response = controller.SubmitParcel(null);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            var typedResponse = (BadRequestObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(400, statusCode);
        }
    }
}