using System;
using Newtonsoft.Json.Linq;
using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.BusinessLogic.Models
{
    /// <summary>
    /// </summary>
    
    public class Hop
    {
        /// <summary>
        /// Gets or Sets HopType
        /// </summary>
        public string HopType { get; set; }

        /// <summary>
        /// Unique CODE of the hop.
        /// </summary>
        /// <value>Unique CODE of the hop.</value>
        public string Code { get; set; }

        /// <summary>
        /// Description of the hop.
        /// </summary>
        /// <value>Description of the hop.</value>
        public string Description { get; set; }

        /// <summary>
        /// Delay processing takes on the hop.
        /// </summary>
        /// <value>Delay processing takes on the hop.</value>
        public int? ProcessingDelayMins { get; set; }

        /// <summary>
        /// Name of the location (village, city, ..) of the hop.
        /// </summary>
        /// <value>Name of the location (village, city, ..) of the hop.</value>
        public string LocationName { get; set; }

        /// <summary>
        /// Gets or Sets LocationCoordinates
        /// </summary>
        public GeoCoordinate LocationCoordinates { get; set; }
    }

    public class HopJsonConverter : JsonCreationConverter<Hop>
    {
        protected override Hop Create(Type objectType, JObject jObject)
        {
            if (jObject == null) throw new ArgumentNullException("jObject");

            if (jObject["hopType"] != null && (string) jObject["hopType"] is var hopTypeString && hopTypeString != "")
            {
                if (hopTypeString == "Truck")
                {
                    return new Truck();
                }
                else if (hopTypeString == "Transferwarehouse")
                {
                    return new Transferwarehouse();
                } else
                {
                    return new Warehouse();
                }
            }

            return new Hop();
        }
    }
}