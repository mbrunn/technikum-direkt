/*
 * Parcel Logistics Service
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: 1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace TechnikumDirekt.Services.Models
{
    /// <summary>
    /// </summary>
    [DataContract]
    public class Truck : Hop
    {
        /// <summary>
        /// GeoJSON of the are covered by the truck.
        /// </summary>
        /// <value>GeoJSON of the are covered by the truck.</value>
        [Required]
        [DataMember(Name="regionGeoJson")]
        public string RegionGeoJson { get; set; }

        /// <summary>
        /// The truck's number plate.
        /// </summary>
        /// <value>The truck's number plate.</value>
        [Required]
        [DataMember(Name="numberPlate")]
        public string NumberPlate { get; set; }
    }
}