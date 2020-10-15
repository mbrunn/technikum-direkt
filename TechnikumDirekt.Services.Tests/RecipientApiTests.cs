using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TechnikumDirekt.Services.Controllers;

namespace TechnikumDirekt.Services.Tests
{
    [TestFixture]
    public class RecipientApiTests
    {
        [Test]
        public void TrackParcel_ValidParcel_Ok()
        {
            var controller = new RecipientApiController();

            var response = controller.TrackParcel("trackingID123");

            Assert.IsInstanceOf<OkObjectResult>(response);

            object body = ((OkObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 200);
        }
        
        [Test]
        public void TrackParcel_NullParcel_BadRequest()
        {
            var controller = new RecipientApiController();

            var response = controller.TrackParcel(null);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            
            object body = ((BadRequestObjectResult) response).Value;
            
            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 400);
        }
        
        [Test]
        public void TrackParcel_NonExistingParcel_NotFound()
        {
            var controller = new RecipientApiController();

            var response = controller.TrackParcel("NonExistingParcel");

            Assert.IsInstanceOf<NotFoundObjectResult>(response);
            
            object body = ((NotFoundObjectResult) response).Value;
            
            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 404);
        }
    }
}