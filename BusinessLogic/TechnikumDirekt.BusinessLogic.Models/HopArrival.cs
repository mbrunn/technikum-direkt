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

namespace TechnikumDirekt.BusinessLogic.Models
{
    /// <summary>
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

        public DateTime? DateTime { get; set; }
    }
}