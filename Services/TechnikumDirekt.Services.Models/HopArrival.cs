/*
 * Parcel Logistics Service
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: 1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace TechnikumDirekt.Services.Models
{
    /// <summary>
    /// This is used to track where a parcel has been or is heading.
    /// </summary>
    /// 
    [DataContract]
    public class HopArrival
    {
        /// <summary>
        ///     Unique CODE of the hop.
        /// </summary>
        /// <value>Unique CODE of the hop.</value>
        [Required]
        [DataMember(Name = "code")]
        public string Code { get; set; }

        /// <summary>
        ///     Description of the hop.
        /// </summary>
        /// <value>Description of the hop.</value>
        [Required]
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        ///     The date/time the parcel arrived at the hop.
        /// </summary>
        /// <value>The date/time the parcel arrived at the hop.</value>
        [Required]
        [DataMember(Name = "dateTime")]
        public DateTime? DateTime { get; set; }
    }
}