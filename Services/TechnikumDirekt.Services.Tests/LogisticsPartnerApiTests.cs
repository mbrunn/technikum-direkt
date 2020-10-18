using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.Services.Controllers;
using TechnikumDirekt.Services.Mapper;
using BlParcel = TechnikumDirekt.BusinessLogic.Models.Parcel;
using BlRecipient = TechnikumDirekt.BusinessLogic.Models.Recipient;
using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Services.Tests
{
    [TestFixture]
    public class LogisticsPartnerApiTests
    {
        private ITrackingLogic _trackingLogic;
        private IMapper _mapper;
        
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
            // Setup - TransitionParcelFromPartner
            mockTrackingLogic.Setup(m => m.TransitionParcelFromPartner(validParcel, ValidTrackingNumber));
            mockTrackingLogic.Setup(m => m.TransitionParcelFromPartner(It.IsAny<BlParcel>(), InvalidTrackingNumber)).Throws(new ValidationException(""));
            mockTrackingLogic.Setup(m => m.TransitionParcelFromPartner(null, It.IsAny<string>())).Throws(new ValidationException(""));

            _trackingLogic = mockTrackingLogic.Object;
        }
        
        [Test]
        public void TransitionParcel_ValidTrackingID_OkRequest()
        {
            var controller = new LogisticsPartnerApiController(_trackingLogic, _mapper);
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2
            };

            var response = controller.TransitionParcel(parcel, ValidTrackingNumber);
            
            Assert.IsInstanceOf<OkObjectResult>(response);

            var typedResponse = (OkObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(200, statusCode);
        }

        [Test]
        public void TransitionParcel_nullParcel_BadRequest()
        {
            var controller = new LogisticsPartnerApiController(_trackingLogic, _mapper);

            var response = controller.TransitionParcel(null, ValidTrackingNumber);
            
            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            var typedResponse = (BadRequestObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(400, statusCode);
        }

        [Test]
        public void TransitionParcel_invalidTrackingID_BadRequest()
        {
            var controller = new LogisticsPartnerApiController(_trackingLogic, _mapper);
            var parcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2
            };

            var response = controller.TransitionParcel(parcel, InvalidTrackingNumber);
            
            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            var typedResponse = (BadRequestObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(400, statusCode);
        }
    }
}