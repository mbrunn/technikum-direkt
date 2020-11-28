using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using TechnikumDirekt.ServiceAgents.Exceptions;
using TechnikumDirekt.ServiceAgents.Interfaces;
using TechnikumDirekt.ServiceAgents.Models;
using TechnikumDirekt.Services.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TechnikumDirekt.ServiceAgents
{
    public class TransferParcelToPartnerAgent : ILogisticsPartnerAgent
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<TransferParcelToPartnerAgent> _logger;

        public TransferParcelToPartnerAgent(IHttpClientFactory clientFactory, ILogger<TransferParcelToPartnerAgent> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }
        
        /// <summary>
        /// Transitions a given parcel to the given transferWarehouse
        /// </summary>
        /// <param name="parcel">
        /// Parcel that should be transitioned
        /// </param>
        /// <param name="transferWarehouse">
        /// Transferwarehouse the parcel should be transitioned to.
        /// </param>
        public bool TransitionParcelToPartner(string trackingId, Parcel parcel, Transferwarehouse transferWarehouse)
        {
            //Call API here:
            if(trackingId == null || parcel == null || transferWarehouse == null) throw new ArgumentNullException();
            
            var jsonContent =
                new StringContent(JsonSerializer.Serialize(parcel), Encoding.UTF8, "application/json");
            var client = _clientFactory.CreateClient("logisticsPartner");

            var response = client
                .PostAsync($"{transferWarehouse.LogisticsPartnerUrl}/parcel/{trackingId}", jsonContent).Result;

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogDebug(
                    $"Unsuccessful response from {transferWarehouse.LogisticsPartnerUrl}: {response.StatusCode}");
                throw new ServiceAgentsBadResponseException(
                    $"Unsuccessful response from {transferWarehouse.LogisticsPartnerUrl}: {response.StatusCode}");
            }

            _logger.LogDebug($"Successful response from {transferWarehouse.LogisticsPartnerUrl}.");

            return true;
        }
    }
}