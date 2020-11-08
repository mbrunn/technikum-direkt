using AutoMapper;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
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

            var feature = new Feature {Geometry = sourceMember};
            var writer = new GeoJsonWriter();
            var geoJsonString = writer.Write(feature);

            return geoJsonString;
        }
    }
}