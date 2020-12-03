using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TechnikumDirekt.BusinessLogic.Models;
using TechnikumDirekt.ServiceAgents.Exceptions;
using TechnikumDirekt.ServiceAgents.Interfaces;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TechnikumDirekt.ServiceAgents
{
    public class WebhookServiceAgent: IWebhookServiceAgent
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<WebhookServiceAgent> _logger;
        
        public WebhookServiceAgent(IHttpClientFactory clientFactory, ILogger<WebhookServiceAgent> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }
        
        /// <summary>
        /// Notifies the URL of the webhook with given Information
        /// </summary>
        /// <param name="webhook"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void NotifySubscriber(Webhook webhook)
        {
            if(webhook.Parcel == null ||  string.IsNullOrEmpty(webhook.Url)) throw new ArgumentNullException();
            
            WebhookMessage webhookMsg;
            
            try
            {
                var parcelState = webhook.Parcel.State.ToString();
                parcelState = parcelState.Remove(parcelState.IndexOf("Enum", StringComparison.Ordinal));
                
                webhookMsg = new WebhookMessage()
                {
                    state = parcelState,
                    visitedHops = JsonSerializer.Serialize(webhook.Parcel.VisitedHops),
                    futureHops = JsonSerializer.Serialize(webhook.Parcel.FutureHops),
                    trackingId = webhook.Parcel.TrackingId
                };
            }
            catch (Exception e)
            {
                _logger.LogDebug($"Webhook has invalid Data.");
                throw new ServiceAgentsBadArgumentException($"Webhook has invalid Data.", e);
            }

            var stringContent = JsonConvert.SerializeObject(webhookMsg);
            
            var jsonContent = new StringContent(stringContent, Encoding.UTF8, "application/json");
            
            var client = _clientFactory.CreateClient("webhookNotifier"); //TODO register this shit
            
            var response = client
                .PostAsync($"{webhook.Url}", jsonContent).Result;

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogDebug(
                    $"Unsuccessful response from {webhook.Url}: {response.StatusCode}");
                throw new ServiceAgentsBadResponseException(
                    $"Unsuccessful response from {webhook.Url}: {response.StatusCode}");
            }

            _logger.LogDebug($"Successful response from webhook URL: {webhook.Url}.");
            return;
        }
        
        internal class WebhookMessage
        {
            public string state { get; set; }
            public string visitedHops { get; set; }
            public string futureHops { get; set; }
            public string trackingId { get; set; }
        }
    }
}