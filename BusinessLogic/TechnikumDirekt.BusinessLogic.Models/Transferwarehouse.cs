using NetTopologySuite.Geometries;

namespace TechnikumDirekt.BusinessLogic.Models
{
    /// <summary>
    /// </summary>
    public class Transferwarehouse : Hop
    {
        /// <summary>
        /// GeoJSON of the area covered by the logistics partner.
        /// </summary>
        /// <value>GeoJSON of the area covered by the logistics partner.</value>
        public Geometry RegionGeometry { get; set; }

        /// <summary>
        /// Name of the logistics partner.
        /// </summary>
        /// <value>Name of the logistics partner.</value>
        public string LogisticsPartner { get; set; }

        /// <summary>
        /// BaseURL of the logistics partner&#x27;s REST service.
        /// </summary>
        /// <value>BaseURL of the logistics partner&#x27;s REST service.</value>
        public string LogisticsPartnerUrl { get; set; }
    }
}