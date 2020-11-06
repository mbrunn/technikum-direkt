using AutoMapper;
using Castle.Core.Logging;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.Services.Controllers;
using TechnikumDirekt.Services.Mapper;
using BlParcel = TechnikumDirekt.BusinessLogic.Models.Parcel;
using BlRecipient = TechnikumDirekt.BusinessLogic.Models.Recipient;
using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Services.Tests
{
    [TestFixture]
    public class StaffApiTests
    {
        private ITrackingLogic _trackingLogic;
        private NullLogger<StaffApiController> _logger;

        private const string ValidTrackingNumber = "A123BCD23";
        private const string InvalidTrackingNumber = "A123BaD23";
        private const string NotfoundTrackingNumber = "000000000";
        private const string ValidHopCode = "ABCD1234";
        private const string InvalidHopCode = "AbdA2a";
        private const string NotfoundHopCode = "ABCD0000";
        
        [OneTimeSetUp]
        public void Setup()
        {
            var mockTrackingLogic = new Mock<ITrackingLogic>();
            // Setup - ReportParcelDelivery
            mockTrackingLogic.Setup(m => m.ReportParcelDelivery(ValidTrackingNumber));
            mockTrackingLogic.Setup(m => m.ReportParcelDelivery(InvalidTrackingNumber)).Throws(new ValidationException(""));
            mockTrackingLogic.Setup(m => m.ReportParcelDelivery(NotfoundTrackingNumber)).Throws<TrackingLogicException>();
            // Setup - ReportParcelHop
            mockTrackingLogic.Setup(m => m.ReportParcelHop(ValidTrackingNumber, ValidHopCode));
            mockTrackingLogic.Setup(m => m.ReportParcelHop(InvalidTrackingNumber, It.IsAny<string>())).Throws(new ValidationException(""));
            mockTrackingLogic.Setup(m => m.ReportParcelHop(It.IsAny<string>(), InvalidHopCode)).Throws(new ValidationException(""));
            mockTrackingLogic.Setup(m => m.ReportParcelHop(NotfoundTrackingNumber, It.IsAny<string>())).Throws<TrackingLogicException>();
            mockTrackingLogic.Setup(m => m.ReportParcelHop(It.IsAny<string>(), NotfoundHopCode)).Throws<TrackingLogicException>();

            _trackingLogic = mockTrackingLogic.Object;
            _logger = NullLogger<StaffApiController>.Instance;
        }

        #region ReportParcelDelivery Tests

        [Test]
        public void ReportParcelDelivery_ValidTrackingId_Ok()
        {
            var controller = new StaffApiController(_trackingLogic, _logger);
            
            var response = controller.ReportParcelDelivery(ValidTrackingNumber);

            Assert.IsInstanceOf<OkObjectResult>(response);

            var typedResponse = (OkObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(200, statusCode);
        }
        
        [Test]
        public void ReportParcelDelivery_NonexistentTrackingId_NotFound()
        {
            var controller = new StaffApiController(_trackingLogic, _logger);

            var response = controller.ReportParcelDelivery(NotfoundTrackingNumber);

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            var typedResponse = (NotFoundObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(404, statusCode);
        }
        
        [Test]
        public void ReportParcelDelivery_InvalidTrackingId_BadRequest()
        {
            var controller = new StaffApiController(_trackingLogic, _logger);

            var response = controller.ReportParcelDelivery(InvalidTrackingNumber);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            var typedResponse = (BadRequestObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(400, statusCode);
        }

        #endregion

        #region ReportParcelHop Tests
        
        [Test]
        public void ReportParcelHop_ValidTrackingID_OkRequest()
        {
            var controller = new StaffApiController(_trackingLogic, _logger);
            
            var response = controller.ReportParcelHop(ValidTrackingNumber, ValidHopCode);

            Assert.IsInstanceOf<OkObjectResult>(response);

            var typedResponse = (OkObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(200, statusCode);
        }

        [Test]
        public void ReportParcelHop_InvalidHopCode_BadRequest()
        {
            var controller = new StaffApiController(_trackingLogic, _logger);
            
            var response = controller.ReportParcelHop(ValidTrackingNumber, InvalidHopCode);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            var typedResponse = (BadRequestObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(400, statusCode);
        }

        [Test]
        public void ReportParcelHop_InvalidTrackingID_BadRequest()
        {
            var controller = new StaffApiController(_trackingLogic, _logger);
            
            var response = controller.ReportParcelHop(InvalidTrackingNumber, ValidHopCode);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            var typedResponse = (BadRequestObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(400, statusCode);
        }
        
        [Test]
        public void ReportParcelHop_InvalidTrackingIDandHopCode_BadRequest()
        {
            var controller = new StaffApiController(_trackingLogic, _logger);
            
            var response = controller.ReportParcelHop(InvalidTrackingNumber, InvalidHopCode);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            var typedResponse = (BadRequestObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(400, statusCode);
        }
        
        [Test]
        public void ReportParcelHop_NonExistingTrackingId_NotFound()
        {
            var controller = new StaffApiController(_trackingLogic, _logger);
            
            var response = controller.ReportParcelHop(NotfoundTrackingNumber, ValidHopCode);

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            var typedResponse = (NotFoundObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(404, statusCode);
        }
        
        [Test]
        public void ReportParcelHop_NonExistingHopCode_NotFound()
        {
            var controller = new StaffApiController(_trackingLogic, _logger);
            
            var response = controller.ReportParcelHop(ValidTrackingNumber, NotfoundHopCode);

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            var typedResponse = (NotFoundObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(404, statusCode);
        }
        
        #endregion
    }
}