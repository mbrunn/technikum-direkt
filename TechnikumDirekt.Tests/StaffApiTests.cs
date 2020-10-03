using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TechnikumDirekt.Services.Controllers;

namespace TechnikumDirekt.Tests
{
    [TestFixture]
    public class StaffApiTests
    {
        [Test]
        public void ReportParcelDelivery_ValidTrackingId_Ok()
        {
            var controller = new StaffApiController();
            
            var response = controller.ReportParcelDelivery("123");

            Assert.IsInstanceOf<OkObjectResult>(response);

            var body = ((OkObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 200);
        }
        
        [Test]
        public void ReportParcelDelivery_InValidTrackingId_NotFound()
        {
            var controller = new StaffApiController();

            var response = controller.ReportParcelDelivery("NonExistingParcel");

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            var body = ((NotFoundObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 404);
        }
        
        [Test]
        public void ReportParcelDelivery_NullTrackingId_BadRequest()
        {
            var controller = new StaffApiController();

            var response = controller.ReportParcelDelivery(null);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            object body = ((BadRequestObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 400);
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [Test]
        public void ReportParcelHop_ValidTrackingID_OkRequest()
        {
            var controller = new StaffApiController();
           
            var response = controller.ReportParcelHop("12345", "HopCode123");

            Assert.IsInstanceOf<OkObjectResult>(response);

            object body = ((OkObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 200);
        }

        [Test]
        public void ReportParcelHop_nullHopCode_BadRequest()
        {
            var controller = new StaffApiController();
            
            var response = controller.ReportParcelHop("trackingID", null);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            object body = ((BadRequestObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 400);
        }

        [Test]
        public void ReportParcelHop_nullTrackingID_BadRequest()
        {
            var controller = new StaffApiController();
            
            var response = controller.ReportParcelHop(null, "HopCode");

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            object body = ((BadRequestObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 400);
        }
        
        [Test]
        public void ReportParcelHop_nullTrackingIDandHopCode_BadRequest()
        {
            var controller = new StaffApiController();
            
            var response = controller.ReportParcelHop(null, null);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            object body = ((BadRequestObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 400);
        }
        
        [Test]
        public void ReportParcelHop_NonExistingTrackingId_NotFound()
        {
            var controller = new StaffApiController();
            
            var response = controller.ReportParcelHop("NonExistingTrackingId", "HopCode");

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            object body = ((NotFoundObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 404);
        }
        
        [Test]
        public void ReportParcelHop_NonExistingHopCode_NotFound()
        {
            var controller = new StaffApiController();
            
            var response = controller.ReportParcelHop("12345", "NonExistingHopCode");

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            object body = ((NotFoundObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 404);
        }
    }
}