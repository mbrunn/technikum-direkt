using NUnit.Framework;

namespace TechnikumDirekt.Services.Tests
{
    [TestFixture]
    public class SenderApiTests
    {/*
        [Test]
        public void SubmitParcel_ValidParcel_Ok()
        {
            var controller = new SenderApiController();
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
                Recipient = new Recipient(){
                    Name = "Benji Bananas",
                    Street = "Banana Street 2",
                    PostalCode = "4242",
                    City = "Banana City",
                    Country = "AT"
                }
            };

            var response = controller.SubmitParcel(parcel);

            Assert.IsInstanceOf<OkObjectResult>(response);

            object body = ((OkObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 200);
        }
        
        [Test]
        public void SubmitParcel_NullParcel_BadRequest()
        {
            var controller = new SenderApiController();

            var response = controller.SubmitParcel(null);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            
            object body = ((BadRequestObjectResult) response).Value;
            
            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 400);
            
        }*/
    }
}