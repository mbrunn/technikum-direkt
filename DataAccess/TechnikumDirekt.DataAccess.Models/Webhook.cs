using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechnikumDirekt.DataAccess.Models
{
    public class Webhook
    {
        /// <summary>
        /// PK
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Parcel that is tracked by this webhook
        /// </summary>
        public Parcel Parcel { get; set; }
     
        /// <summary>
        /// Url to call if ParcelState changes
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// Time of subscription
        /// </summary>
        public DateTime? CreationDate { get; set; }
    }
}