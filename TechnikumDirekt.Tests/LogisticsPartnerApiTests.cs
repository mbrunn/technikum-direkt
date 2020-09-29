using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TechnikumDirekt.Services.Controllers;
using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Tests
{
    [TestFixture]
    public class LogisticsPartnerApiTests
    {
        [Test]
        public void TransitionParcel_ValidTrackingID_OkRequest()
        {
            var controller = new LogisticsPartnerApiController();
            var parcel = new Parcel()
            {
                Weight = 2.0f,
                Sender = new Recipient()
                {
                    Name = "Michi Mango",
                    Street = "TestStreet 1",
                    PostalCode = "1234",
                    City = "Mistelbach Weltstadt",
                    Country = "AT"
                },
                Recipient = new Recipient()
                {
                    Name = "Benji Bananas",
                    Street = "Banana Street 2",
                    PostalCode = "4242",
                    City = "Banana City",
                    Country = "AT"
                }
            };

            var response = controller.TransitionParcel(parcel, "trackingID123");

            Assert.IsInstanceOf<OkObjectResult>(response);

            object body = ((OkObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 200);
        }

        [Test]
        public void TransitionParcel_nullParcel_BadRequest()
        {
            var controller = new LogisticsPartnerApiController();
            Parcel parcel = null;

            var response = controller.TransitionParcel(parcel, "123");

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            object body = ((BadRequestObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 400);
        }

        [Test]
        public void TransitionParcel_nullTrackingID_BadRequest()
        {
            var controller = new LogisticsPartnerApiController();
            var parcel = new Parcel()
            {
                Weight = 2.0f,
                Sender = new Recipient()
                {
                    Name = "Michi Mango",
                    Street = "TestStreet 1",
                    PostalCode = "1234",
                    City = "Mistelbach Weltstadt",
                    Country = "AT"
                },
                Recipient = new Recipient()
                {
                    Name = "Benji Bananas",
                    Street = "Banana Street 2",
                    PostalCode = "4242",
                    City = "Banana City",
                    Country = "AT"
                }
            };

            var response = controller.TransitionParcel(parcel, null);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            object body = ((BadRequestObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 400);
        }
    }
}