using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TechnikumDirekt.ServiceAgents.Exceptions;
using TechnikumDirekt.ServiceAgents.Interfaces;
using TechnikumDirekt.ServiceAgents.Models;

namespace TechnikumDirekt.ServiceAgents
{
    public class OsmGeoEncodingAgent: IGeoEncodingAgent
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<OsmGeoEncodingAgent> _logger;
        public OsmGeoEncodingAgent(IHttpClientFactory clientFactory, ILogger<OsmGeoEncodingAgent> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
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

            //TODO: is this clean ? I don't think so, but it works.
            #region data cleaning
            
            address.PostalCode = address.PostalCode.Substring(address.PostalCode.LastIndexOf('-')+1);
            
            /*if (address.Country.Equals("Österreich"))
            {
                address.Country = "Austria";
            }*/
            
            #endregion
            
            var response = client.GetAsync($"search?q={address.PostalCode}+{address.City}+{address.Street}&format=json").Result;
            
            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                var responseObject = JsonConvert.DeserializeObject<List<OsmSearchResponse>>(responseString);

                if (responseObject != null && responseObject.Count > 0)
                {
                    var lon = responseObject.First().Lon;
                    var lat = responseObject.First().Lat;
                    _logger.LogTrace($"OSM found address with coordinates: Lon: {lon.ToString()} Lat: {lat.ToString()} ");
                    return new Point(responseObject.First().Lon, responseObject.First().Lat) {SRID = 4326};
                }
            }
            else
            {
                _logger.LogTrace($"Unsuccessful response from osm: {response.StatusCode} {response.Content}");
                throw new ServiceAgentsBadResponseException($"Unsuccessful response: {response.StatusCode}");
            }

            _logger.LogTrace($"{address.Country}; {address.PostalCode}; {address.City}; {address.Street}; could not be resolved to GPS coordinates.");
            throw new ServiceAgentsNotFoundException($"Address could not be resolved to GPS coordinates.");
        }
    }

    internal class OsmSearchResponse
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}