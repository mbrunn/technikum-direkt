using GeoCoordinate = GeoCoordinatePortable.GeoCoordinate;

namespace TechnikumDirekt.BusinessLogic.Models
{
    /// <summary>
    /// </summary>
    public class Truck : Hop
    {
        public string NumberPlate { get; set; }
        public GeoCoordinate RegionGeometry { get; set; }
    }
}