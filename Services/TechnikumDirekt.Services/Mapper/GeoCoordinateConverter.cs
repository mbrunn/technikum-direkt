using System.Data;
using AutoMapper;
using GeoCoordinate = GeoCoordinatePortable.GeoCoordinate;
using SvcModels = TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Services.Mapper
{
    public class GeoCoordinateConverter : IValueConverter<SvcModels.GeoCoordinate, GeoCoordinate>, IValueConverter<GeoCoordinate, SvcModels.GeoCoordinate>
    {
        private ResolutionContext _context;

        /// <summary>
        /// Defines Converters for converting SvcModels.GeoCoordinate 
        /// </summary>
        /// <param name="sourceMember"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NoNullAllowedException"></exception>
        public GeoCoordinate Convert(SvcModels.GeoCoordinate sourceMember, ResolutionContext context)
        {
            _context = context;
            if (sourceMember.Lat == null || sourceMember.Lon == null)
            {
                throw new NoNullAllowedException();
            }
            return new GeoCoordinate((double) sourceMember.Lat, (double) sourceMember.Lon);
        }

        public SvcModels.GeoCoordinate Convert(GeoCoordinate sourceMember, ResolutionContext context)
        {
            _context = context;
            return new SvcModels.GeoCoordinate(){Lat = sourceMember.Latitude, Lon = sourceMember.Longitude};
        }
    }
}