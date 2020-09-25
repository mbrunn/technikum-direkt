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
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace technikumDirekt.Services.Models
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class Warehouse : Hop
    { 
        /// <summary>
        /// Gets or Sets GeoCoordinate
        /// </summary>
        [Required]
        [DataMember(Name="geoCoordinate")]
        public GeoCoordinate GeoCoordinate { get; set; }

        /// <summary>
        /// Next hops after this warehouse (warehouses or trucks).
        /// </summary>
        /// <value>Next hops after this warehouse (warehouses or trucks).</value>
        [Required]
        [DataMember(Name="nextHops")]
        public List<WarehouseNextHops> NextHops { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Warehouse {\n");
            sb.Append("  GeoCoordinate: ").Append(GeoCoordinate).Append("\n");
            sb.Append("  NextHops: ").Append(NextHops).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public  new string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Warehouse)obj);
        }

        /// <summary>
        /// Returns true if Warehouse instances are equal
        /// </summary>
        /// <param name="other">Instance of Warehouse to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Warehouse other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    GeoCoordinate == other.GeoCoordinate ||
                    GeoCoordinate != null &&
                    GeoCoordinate.Equals(other.GeoCoordinate)
                ) && 
                (
                    NextHops == other.NextHops ||
                    NextHops != null &&
                    NextHops.SequenceEqual(other.NextHops)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                    if (GeoCoordinate != null)
                    hashCode = hashCode * 59 + GeoCoordinate.GetHashCode();
                    if (NextHops != null)
                    hashCode = hashCode * 59 + NextHops.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(Warehouse left, Warehouse right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Warehouse left, Warehouse right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
