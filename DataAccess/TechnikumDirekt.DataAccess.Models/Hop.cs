using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace TechnikumDirekt.DataAccess.Models
{
    /// <summary>
    /// </summary>
    
    public class Hop
    {
        /// <summary>
        /// Gets or Sets HopType
        /// </summary>
        public HopType HopType { get; set; }

        /// <summary>
        /// Unique CODE of the hop.
        /// </summary>
        /// <value>Unique CODE of the hop.</value>
        [Key]
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
        public Point LocationCoordinates { get; set; }
        
        /// <summary>
        /// Represents HopArrivals for n:m to parcel.
        /// </summary>
        public List<HopArrival> HopArrivals { get; set; }

        /// <summary>
        /// Code of the parent hop.
        /// </summary>
        public string ParentWarehouseCode { get; set; }
        public Warehouse ParentWarehouse { get; set; }
        
        /// <summary>
        /// Travel time from parent hop to this hop.
        /// </summary>
        public int? ParentTraveltimeMins { get; set; }
    }

    public enum HopType
    {
        Warehouse,
        Truck,
        TransferWarehouse
    }
}