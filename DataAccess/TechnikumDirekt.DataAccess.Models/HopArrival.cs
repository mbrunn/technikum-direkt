using System;

namespace TechnikumDirekt.DataAccess.Models
{
    /// <summary>
    /// n:m intermediate table for Parcel <-> Hop
    /// </summary>
    public class HopArrival
    {
        public string ParcelTrackingId { get; set; }
        public Parcel Parcel { get; set; }

        public string HopCode { get; set; }
        public Hop Hop { get; set; }

        /// <summary>
        ///     The date/time the parcel arrived at the hop.
        /// </summary>
        /// <value>The date/time the parcel arrived at the hop.</value>

        public DateTime? HopArrivalTime { get; set; }
        
        public int Order { get; set; }
    }
}