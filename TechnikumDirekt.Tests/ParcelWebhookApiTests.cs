using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TechnikumDirekt.Services.Controllers;
using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Tests
{
    [TestFixture]
    public class ParcelWebhookApiTests
    {
        [Test]
        public void ListParcelWebhooks_ValidTrackingId_Ok()
        {
            var controller = new ParcelWebhookApiController();
            
            var response = controller.ListParcelWebhooks("123");

            Assert.IsInstanceOf<OkObjectResult>(response);

            object body = ((OkObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 200);
        }
        
        [Test]
        public void ListParcelWebhooks_InValidTrackingId_NotFound()
        {
            var controller = new ParcelWebhookApiController();

            var response = controller.ListParcelWebhooks("NonExistingParcel");

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            object body = ((NotFoundObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 404);
        }
        
        [Test]
        public void ListParcelWebhooks_NullTrackingId_NotFound()
        {
            var controller = new ParcelWebhookApiController();

            var response = controller.ListParcelWebhooks(null);

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            object body = ((NotFoundObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 404);
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [Test]
        public void SubscribeParcelWebhook_ValidTrackingIdandUrl_Ok()
        {
            var controller = new ParcelWebhookApiController();

            var response = controller.SubscribeParcelWebhook("12345", "https://12345.at");

            Assert.IsInstanceOf<OkObjectResult>(response);

            object body = ((OkObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 200);
        }
        
        [Test]
        public void SubscribeParcelWebhook_NullTrackingId_NotFound()
        {
            var controller = new ParcelWebhookApiController();

            var response = controller.SubscribeParcelWebhook(null, "https://12345.at");

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            object body = ((NotFoundObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 404);
        }
        
        [Test]
        public void SubscribeParcelWebhook_NullUrl_NotFound()
        {
            var controller = new ParcelWebhookApiController();

            var response = controller.SubscribeParcelWebhook("12345", null);

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            object body = ((NotFoundObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 404);
        }
        
        [Test]
        public void SubscribeParcelWebhook_NonExistingValues_NotFound()
        {
            var controller = new ParcelWebhookApiController();

            var response = controller.SubscribeParcelWebhook("NonExistingParcel", "NonExistingUrl");

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            object body = ((NotFoundObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 404);
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [Test]
        public void UnsubscribeParcelWebhook_ValidId_Ok()
        {
            var controller = new ParcelWebhookApiController();

            var response = controller.UnsubscribeParcelWebhook(1234);

            Assert.IsInstanceOf<OkObjectResult>(response);

            object body = ((OkObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 200);
        }
        
        [Test]
        public void UnsubscribeParcelWebhook_NullId_NotFound()
        {
            var controller = new ParcelWebhookApiController();

            var response = controller.UnsubscribeParcelWebhook(null);

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            object body = ((NotFoundObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 404);
        }
        
        [Test]
        public void UnsubscribeParcelWebhook_NonExistingId_NotFound()
        {
            var controller = new ParcelWebhookApiController();

            var response = controller.UnsubscribeParcelWebhook(4242);

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            object body = ((NotFoundObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 404);
        }
    }
}