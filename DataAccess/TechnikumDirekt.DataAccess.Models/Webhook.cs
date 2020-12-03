﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechnikumDirekt.DataAccess.Models
{
    public class Webhook
    {
        /// <summary>
        /// Id of the subscription
        /// </summary>
        [Key]
        public long Id { get; set; }
        
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