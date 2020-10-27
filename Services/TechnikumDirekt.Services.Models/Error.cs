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
    public class Error
    {
        /// <summary>
        ///     The error message.
        /// </summary>
        /// <value>The error message.</value>
        [Required]
        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }
    }
}