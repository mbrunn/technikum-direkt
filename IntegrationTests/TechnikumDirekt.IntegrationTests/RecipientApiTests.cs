using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.IntegrationTests
{
    [TestFixture]
    public class RecipientApiTests : IntegrationTests, IDisposable
    { 
        private string _datasetLight;
        
        private readonly Recipient _recipient1 = new Recipient
        {
            Name = "Michael Brunner",
            Street = "Dresdner Straße 38-44",
            PostalCode = "1200",
            City = "Wien",
            Country = "AT"
        };

        private readonly Recipient _recipient2 = new Recipient
        {
            Name = "Benjamin Ableidinger",
            Street = "Floridsdorfer Hauptstraße 43",
            PostalCode = "1210",
            City = "Wien",
            Country = "AT"
        };

        private Parcel _validParcel;
        
        private const string NotFoundTrackingId = "A123BCD23";
        private const string InvalidTrackingId = "AXXX";
        
        [OneTimeSetUp]
        public void Setup()
        {
            _datasetLight = Utilities.LoadDatasetLight();
            
            _validParcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2
            };
        }

        #region /parcel/{trackingId} get tests

        [Test]
        public async Task GetTrackingInformation_validTrackingId_OkTrackingInfo()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            var parcelContent = new StringContent(JsonConvert.SerializeObject(_validParcel), Encoding.UTF8, "application/json");
            
            // Act
            await Client.PostAsync("/warehouse", content);
            var parcelResponse = await Client.PostAsync("/parcel", parcelContent);
            var parcelResponseString = await parcelResponse.Content.ReadAsStringAsync();
            var parcelInfo = JsonConvert.DeserializeObject<NewParcelInfo>(parcelResponseString);
            var response = await Client.GetAsync($"/parcel/{parcelInfo.TrackingId}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            
            var responseString = await response.Content.ReadAsStringAsync();
            var trackingInformation = JsonConvert.DeserializeObject<TrackingInformation>(responseString);
            
            Assert.NotNull(trackingInformation);
            Assert.AreEqual(TrackingInformation.StateEnum.PickupEnum, trackingInformation.State);
        }
        
        [Test]
        public async Task GetTrackingInformation_invalidTrackingId_BadRequest()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            
            // Act
            await Client.PostAsync("/warehouse", content);
            var response = await Client.GetAsync($"/parcel/{InvalidTrackingId}");
            
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Test]
        public async Task GetTrackingInformation_notfoundTrackingId_NotFound()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            
            // Act
            await Client.PostAsync("/warehouse", content);
            var response = await Client.GetAsync($"/parcel/{NotFoundTrackingId}");
            
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion
    }
}