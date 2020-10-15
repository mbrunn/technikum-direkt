/*
 * Parcel Logistics Service
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: 1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using NetTopologySuite.Geometries;

namespace TechnikumDirekt.BusinessLogic.Models
{
    /// <summary>
    /// </summary>
    public class Truck : Hop
    {
        public string NumberPlate { get; set; }
        public Geometry RegionGeometry { get; set; }
    }
}