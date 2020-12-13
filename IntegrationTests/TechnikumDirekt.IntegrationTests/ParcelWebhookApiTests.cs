using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.IntegrationTests
{
    [TestFixture]
    public class ParcelWebhookApiTests : IntegrationTests
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

        private const string ValidUrl = "http://www.orf.at";
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
        
        #region /parcel/{trackingId}/webhooks post tests

        [Test]
        public async Task PostWebhook_validTrackingId_OkWebhookResponse()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            var parcelContent = new StringContent(JsonConvert.SerializeObject(_validParcel), Encoding.UTF8, "application/json");

            // Act
            await Client.PostAsync(Client.BaseAddress.AbsoluteUri + "/warehouse", content);
            var parcelResponse = await Client.PostAsync(Client.BaseAddress.AbsoluteUri + "/parcel", parcelContent);
            var parcelResponseString = await parcelResponse.Content.ReadAsStringAsync();
            var parcelInfo = JsonConvert.DeserializeObject<NewParcelInfo>(parcelResponseString);
            var parcelTrackingId = parcelInfo.TrackingId;
            
            var parameters = new Dictionary<string, string>
            {
                { "url", ValidUrl }
            };
            var encodedContent = new FormUrlEncodedContent (parameters);
            
            var response = await Client.PostAsync(Client.BaseAddress.AbsoluteUri + $"/parcel/{parcelTrackingId}/webhooks?url={ValidUrl}", encodedContent);
            
            Assert.NotNull(response);
            
            var webhookResponseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseJson = JsonConvert.DeserializeObject<JObject>(webhookResponseString);

            var webhookResponseJson = responseJson.GetValue("value").ToString();
            
            var webhookResponse = JsonConvert.DeserializeObject<WebhookResponse>(webhookResponseJson);
            
            Assert.NotNull(webhookResponse);
            Assert.AreEqual(parcelTrackingId, webhookResponse.TrackingId);
        }
        
        #endregion

        #region /parcel/{trackingId}/webhooks get tests

        [Test]
        public async Task GetWebhook_validTrackingId_OkWebhookResponses()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            var parcelContent = new StringContent(JsonConvert.SerializeObject(_validParcel), Encoding.UTF8, "application/json");

            // Act
            await Client.PostAsync(Client.BaseAddress.AbsoluteUri + "/warehouse", content);
            var parcelResponse = await Client.PostAsync(Client.BaseAddress.AbsoluteUri + "/parcel", parcelContent);
            var parcelResponseString = await parcelResponse.Content.ReadAsStringAsync();
            var parcelInfo = JsonConvert.DeserializeObject<NewParcelInfo>(parcelResponseString);
            var parcelTrackingId = parcelInfo.TrackingId;
            
            var parameters = new Dictionary<string, string>
            {
                { "url", ValidUrl }
            };
            var encodedContent = new FormUrlEncodedContent (parameters);
            
            var webhookResponse = await Client.PostAsync(Client.BaseAddress.AbsoluteUri +$"/parcel/{parcelTrackingId}/webhooks?url={ValidUrl}", encodedContent);
            
            Assert.NotNull(webhookResponse);
            
            var response = await Client.GetAsync(Client.BaseAddress.AbsoluteUri + $"/parcel/{parcelTrackingId}/webhooks");
            
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            
            var responseString = await response.Content.ReadAsStringAsync();
            
            var responseJson = JsonConvert.DeserializeObject<JObject>(responseString);

            var webhookResponsesJson = responseJson.GetValue("value").ToString();

            var webhookResponses = JsonConvert.DeserializeObject<IEnumerable<WebhookResponse>>(webhookResponsesJson);
            
            Assert.NotNull(webhookResponses);
            Assert.AreEqual(parcelTrackingId, webhookResponses.FirstOrDefault(wh => wh.TrackingId == parcelTrackingId)?.TrackingId);
        }

        #endregion
        
        #region /parcel/webhooks/{id} delete tests

        [Test]
        public async Task DeleteWebhook_validTrackingId_OkWebhookResponse()
        {
            // Arrange
            var content = new StringContent(_datasetLight, Encoding.UTF8, "application/json");
            var parcelContent = new StringContent(JsonConvert.SerializeObject(_validParcel), Encoding.UTF8, "application/json");

            // Act
            await Client.PostAsync(Client.BaseAddress.AbsoluteUri + "/warehouse", content);
            var parcelResponse = await Client.PostAsync(Client.BaseAddress.AbsoluteUri + "/parcel", parcelContent);
            var parcelResponseString = await parcelResponse.Content.ReadAsStringAsync();
            var parcelInfo = JsonConvert.DeserializeObject<NewParcelInfo>(parcelResponseString);
            var parcelTrackingId = parcelInfo.TrackingId;
            
            var parameters = new Dictionary<string, string>
            {
                { "url", ValidUrl }
            };
            var encodedContent = new FormUrlEncodedContent (parameters);
            
            var response = await Client.PostAsync(Client.BaseAddress.AbsoluteUri +$"/parcel/{parcelTrackingId}/webhooks?url={ValidUrl}", encodedContent);
            
            Assert.NotNull(response);
            
            var webhookResponseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseJson = JsonConvert.DeserializeObject<JObject>(webhookResponseString);

            var webhookResponseJson = responseJson.GetValue("value").ToString();
            
            var webhookResponse = JsonConvert.DeserializeObject<WebhookResponse>(webhookResponseJson);
            
            Assert.NotNull(webhookResponse);

            var webhookId = webhookResponse.Id;

            response = await Client.DeleteAsync($"/parcel/webhooks/{webhookId}");
            
            // Assert
            response.EnsureSuccessStatusCode();
        }

        #endregion
    }
}