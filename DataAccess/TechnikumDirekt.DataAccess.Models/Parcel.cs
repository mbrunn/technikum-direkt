using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechnikumDirekt.DataAccess.Models
{
    public class Parcel
    {
        public enum StateEnum
        {
            /// <summary>
            /// Enum PickupEnum for Pickup
            /// </summary>
            PickupEnum = 0,

            /// <summary>
            /// Enum InTransportEnum for InTransport
            /// </summary>
            InTransportEnum = 1,

            /// <summary>
            /// Enum InTruckDeliveryEnum for InTruckDelivery
            /// </summary>
            InTruckDeliveryEnum = 2,

            /// <summary>
            /// Enum TransferredEnum for Transferred
            /// </summary>
            TransferredEnum = 3,

            /// <summary>
            /// Enum DeliveredEnum for Delivered
            /// </summary>
            DeliveredEnum = 4
        }

        /// <summary>
        /// Parcel tracking id, pk.
        /// </summary>
        [Key]
        public string TrackingId { get; set; }

        /// <summary>
        ///     State of the parcel.
        /// </summary>
        /// <value>State of the parcel.</value>
        public StateEnum? State { get; set; }

        /// <summary>
        ///     Gets or Sets Weight
        /// </summary>
        public float? Weight { get; set; }

        /// <summary>
        ///     Gets or Sets Recipient
        /// </summary>
        public Recipient Recipient { get; set; }

        /// <summary>
        ///     Gets or Sets Sender
        /// </summary>
        public Recipient Sender { get; set; }

        /// <summary>
        ///     Hops visited in the past and future.
        /// </summary>
        /// <value>Hops visited in the past.</value>
        public List<HopArrival> HopArrivals { get; set; }
    }
}