using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using TechnikumDirekt.ServiceAgents.Exceptions;
using TechnikumDirekt.ServiceAgents.Interfaces;
using TechnikumDirekt.ServiceAgents.Models;

namespace TechnikumDirekt.ServiceAgents
{
    public class OsmGeoEncodingAgent: IGeoEncodingAgent
    {
        private readonly IHttpClientFactory _clientFactory;
        
        public OsmGeoEncodingAgent(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// <summary>
        /// Calls a service that converts the address to a location (lat + lon).
        /// </summary>
        /// <param name="address">Input address</param>
        /// <returns>
        ///    Point: result point
        ///    null:  no result found
        /// </returns>
        public Point EncodeAddress(Address address)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            var client = _clientFactory.CreateClient("osm");

            //TODO: is this clean enough ?
            address.PostalCode = address.PostalCode.Substring(address.PostalCode.LastIndexOf('-')+1);
            
            var response = client.GetAsync($"search?q={address.Country}+{address.PostalCode}+{address.City}+{address.Street}&format=json").Result;
            
            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                var responseObject = JsonConvert.DeserializeObject<List<OsmSearchResponse>>(responseString);

                if (responseObject != null && responseObject.Count > 0)
                {
                    return new Point(responseObject.First().Lon, responseObject.First().Lat) {SRID = 4326};
                }
            }
            else
            {
                throw new ServiceAgentsBadResponseException($"Unsuccessful response: {response.StatusCode}");
            }

            throw new ServiceAgentsNotFoundException($"Address could not be resolved to GPS coordinates.");
        }
    }

    internal class OsmSearchResponse
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}