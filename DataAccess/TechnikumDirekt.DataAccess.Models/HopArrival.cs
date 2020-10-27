using System;

namespace TechnikumDirekt.DataAccess.Models
{
    /// <summary>
    /// This is used to track where a parcel has been or is heading.
    /// </summary>

    public class HopArrival
    {
        /// <summary>
        ///     Unique CODE of the hop.
        /// </summary>
        /// <value>Unique CODE of the hop.</value>

        public string Code { get; set; }

        /// <summary>
        ///     Description of the hop.
        /// </summary>
        /// <value>Description of the hop.</value>

        public string Description { get; set; }

        /// <summary>
        ///     The date/time the parcel arrived at the hop.
        /// </summary>
        /// <value>The date/time the parcel arrived at the hop.</value>

        public DateTime? HopArrivalTime { get; set; }
    }
}