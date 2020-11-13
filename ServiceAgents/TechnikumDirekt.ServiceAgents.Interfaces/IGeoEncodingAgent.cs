using NetTopologySuite.Geometries;
using TechnikumDirekt.ServiceAgents.Models;

namespace TechnikumDirekt.ServiceAgents.Interfaces
{
    public interface IGeoEncodingAgent
    {
        /// <summary>
        /// Calls a service that converts the address to a location (lat + lon).
        /// </summary>
        /// <param name="address">Input address</param>
        /// <returns>
        ///    Point: result point
        ///    null:  no result found
        /// </returns>
        Point EncodeAddress(Address address);
    }
}