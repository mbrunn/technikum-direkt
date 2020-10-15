using System.Collections.Generic;
using AutoMapper;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.IO;
using SvcModels = TechnikumDirekt.Services.Models;
using BlModels = TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.Services.Mapper
{
    public class GeoJsonConverter : IValueConverter<string, Geometry>, IValueConverter<Geometry, string>
    {
        private ResolutionContext _context;
        public Geometry Convert(string sourceMember, ResolutionContext context)
        {
            _context = context;
            
            var reader = new GeoJsonReader();
            var featureCollection = reader.Read<Feature>(sourceMember);

            return featureCollection.Geometry;
        }

        public string Convert(Geometry sourceMember, ResolutionContext context)
        {
            _context = context;
            
            var writer = new GeoJsonWriter();
            var featureCollection = writer.Write(sourceMember);

            return featureCollection;
        }
    }
}