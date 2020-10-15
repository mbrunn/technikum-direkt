using System.Data;
using AutoMapper;
using Newtonsoft.Json;
using GeoCoordinate = GeoCoordinatePortable.GeoCoordinate;
using SvcModels = TechnikumDirekt.Services.Models;
using BlModels = TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.Services.Mapper
{
    public class GeoJsonConverter : IValueConverter<string, GeoCoordinate>, IValueConverter<GeoCoordinate, string>
    {
        private ResolutionContext _context;

        public GeoCoordinate Convert(string sourceMember, ResolutionContext context)
        {
            _context = context;

            return null;
        }

        public string Convert(GeoCoordinate sourceMember, ResolutionContext context)
        {
            return null;
        }
    }
}