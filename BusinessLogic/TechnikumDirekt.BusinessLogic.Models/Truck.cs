using NetTopologySuite.Geometries;

namespace TechnikumDirekt.BusinessLogic.Models
{
    /// <summary>
    /// </summary>
    public class Truck : Hop
    {
        /// <summary>
        /// GeoJSON of the area covered by the truck.
        /// </summary>
        /// <value>GeoJSON of the area covered by the truck.</value>
        public Geometry RegionGeometry { get; set; }
        
        /// <summary>
        /// The truck's number plate.
        /// </summary>
        /// <value>The truck's number plate.</value>
        public string NumberPlate { get; set; }
    }
}