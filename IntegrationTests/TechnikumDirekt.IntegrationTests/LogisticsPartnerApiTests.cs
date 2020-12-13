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
    public class LogisticsPartnerApiTests : IntegrationTests, IDisposable
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

        private const string ValidTrackingId = "A123BCD23";
        
        private Parcel _validParcel;
        private Parcel _invalidParcel;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _datasetLight = Utilities.LoadDatasetLight();
            
            _validParcel = new Parcel
            {
                Weight = 2.0f,
                Sender = _recipient1,
                Recipient = _recipient2,
            };
            
            _invalidParcel = new Parcel
            {
                Weight = -2.0f,
                Sender = _recipient1,
                Recipient = _recipient2
            };
        }

        [Test]
        public async Task PostTransferParcelFromPartner_validParcelValidTrackingId_OkValidTrackingId()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            var parcelContent = new StringContent(JsonConvert.SerializeObject(_validParcel), Encoding.UTF8, "application/json");
            
            // Act
            await Client.PostAsync("warehouse", content);
            var response = await Client.PostAsync($"parcel/{ValidTrackingId}", parcelContent);
            
            // Assert
            response.EnsureSuccessStatusCode();
        }
        
        [Test]
        public async Task PostTransferParcelFromPartner_validParcelTrackingIdAlreadyTaken_BadRequest()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            var parcelContent = new StringContent(JsonConvert.SerializeObject(_validParcel), Encoding.UTF8, "application/json");
            
            // Act
            await Client.PostAsync("warehouse", content);
            await Client.PostAsync($"parcel/{ValidTrackingId}", parcelContent);
            var response = await Client.PostAsync($"parcel/{ValidTrackingId}", parcelContent);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}