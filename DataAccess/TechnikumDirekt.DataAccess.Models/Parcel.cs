﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechnikumDirekt.DataAccess.Models
{
    [Table("Parcels", Schema = "Application")]
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
        /// TrackingId of the parcel
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
        public string RecipientId { get; set; } //TODO createId for Recipient
        
        /// <summary>
        ///     Gets or Sets Sender
        /// </summary>
        public string SenderId { get; set; } //TODO createId for Recipient
        
        /// <summary>
        ///     Hops visited in the past.
        /// </summary>
        /// <value>Hops visited in the past.</value>
        public List<HopArrival> VisitedHops { get; set; } //TODO not compatible with database
        
        /// <summary>
        ///     Hops coming up in the future - their times are estimations.
        /// </summary>
        /// <value>Hops coming up in the future - their times are estimations.</value>
        public List<HopArrival> FutureHops { get; set; } //TODO not compatible with database
    }
}