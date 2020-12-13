using System;
using System.Linq;
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
    public class StaffApiTests : IntegrationTests
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
        private const string ValidHopCode = "WENA03";
        
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

        #region /parcel/{trackingId}/reportDelivery post tests

        [Test]
        public async Task PostReportDelivery_validTrackingId_SetsParcelStateToDelivered()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            var parcelContent = new StringContent(JsonConvert.SerializeObject(_validParcel), Encoding.UTF8, "application/json");
            
            // Act
            await Client.PostAsync("warehouse", content);
            var parcelResponse = await Client.PostAsync("parcel", parcelContent);
            var parcelResponseString = await parcelResponse.Content.ReadAsStringAsync();
            var parcelInfo = JsonConvert.DeserializeObject<NewParcelInfo>(parcelResponseString);

            var response = await Client.PostAsync($"parcel/{parcelInfo.TrackingId}/reportDelivery", null);
            var infoResponse = await Client.GetAsync($"parcel/{parcelInfo.TrackingId}");
            
            var responseString = await infoResponse.Content.ReadAsStringAsync();
            var trackingInformation = JsonConvert.DeserializeObject<TrackingInformation>(responseString);
            
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(TrackingInformation.StateEnum.DeliveredEnum, trackingInformation.State);
        }
        
        [Test]
        public async Task PostReportDelivery_invalidTrackingId_BadRequest()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            
            // Act
            await Client.PostAsync("warehouse", content);
            var response = await Client.PostAsync($"parcel/{InvalidTrackingId}/reportDelivery", null);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Test]
        public async Task PostReportDelivery_notfoundTrackingId_NotFound()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            
            // Act
            await Client.PostAsync("warehouse", content);
            var response = await Client.PostAsync($"parcel/{NotFoundTrackingId}/reportDelivery", null);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region /parcel/{trackingId}/reportHop/{code} post tests

        [Test]
        public async Task PostReportParcelHop_validTrackingIdAndHopCode_SetsParcelStateToInTransport()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            var parcelContent = new StringContent(JsonConvert.SerializeObject(_validParcel), Encoding.UTF8, "application/json");
            
            // Act
            await Client.PostAsync("warehouse", content);
            var parcelResponse = await Client.PostAsync("parcel", parcelContent);
            var parcelResponseString = await parcelResponse.Content.ReadAsStringAsync();
            var parcelInfo = JsonConvert.DeserializeObject<NewParcelInfo>(parcelResponseString);
            
            var infoResponse = await Client.GetAsync($"parcel/{parcelInfo.TrackingId}");
            
            var responseString = await infoResponse.Content.ReadAsStringAsync();
            var trackingInformation = JsonConvert.DeserializeObject<TrackingInformation>(responseString);
            var response = await Client.PostAsync($"parcel/{parcelInfo.TrackingId}/reportHop/{trackingInformation.FutureHops.First().Code}", null);
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            infoResponse = await Client.GetAsync($"parcel/{parcelInfo.TrackingId}");
            
            responseString = await infoResponse.Content.ReadAsStringAsync();
            trackingInformation = JsonConvert.DeserializeObject<TrackingInformation>(responseString);
            
            Assert.AreEqual(TrackingInformation.StateEnum.InTransportEnum, trackingInformation.State);
        }

        #endregion
    }
}