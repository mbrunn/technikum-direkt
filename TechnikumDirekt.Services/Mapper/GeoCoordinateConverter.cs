using System.Data;
using AutoMapper;
using GeoCoordinate = GeoCoordinatePortable.GeoCoordinate;
using SvcModels = TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Services.Mapper
{
    public class GeoCoordinateConverter : IValueConverter<Models.GeoCoordinate, GeoCoordinate>, IValueConverter<GeoCoordinate, Models.GeoCoordinate>
    {
        private ResolutionContext _context;

        public GeoCoordinate Convert(Models.GeoCoordinate sourceMember, ResolutionContext context)
        {
            _context = context;
            if (sourceMember.Lat == null || sourceMember.Lon == null)
            {
                throw new NoNullAllowedException();
            }
            return new GeoCoordinate((double) sourceMember.Lat, (double) sourceMember.Lon);
        }

        public Models.GeoCoordinate Convert(GeoCoordinate sourceMember, ResolutionContext context)
        {
            _context = context;
            //return new SvcModels.GeoCoordinate(sourceMember.Latitude, sourceMember.Longitude);
            return new SvcModels.GeoCoordinate();
        }
    }
}