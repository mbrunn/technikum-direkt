using System.Data;
using AutoMapper;
using NetTopologySuite.Geometries;
using SvcModels = TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Services.Mapper
{
    public class PointConverter : IValueConverter<SvcModels.GeoCoordinate, Point>, IValueConverter<Point, SvcModels.GeoCoordinate>
    {
        private ResolutionContext _context;

        /// <summary>
        /// Defines Converters for converting SvcModels.GeoCoordinate 
        /// </summary>
        /// <param name="sourceMember"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NoNullAllowedException"></exception>
        /*
         
         IValueConverter<SvcModels.GeoCoordinate, GeoCoordinate>, IValueConverter<GeoCoordinate, SvcModels.GeoCoordinate>, 
         
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
        }*/

        Point IValueConverter<SvcModels.GeoCoordinate, Point>.Convert(SvcModels.GeoCoordinate sourceMember, ResolutionContext context)
        {
            _context = context;
            if (sourceMember.Lat == null || sourceMember.Lon == null)
            {
                throw new NoNullAllowedException();
            }

            return new Point((double) sourceMember.Lon, (double) sourceMember.Lat);
        }

        public SvcModels.GeoCoordinate Convert(Point sourceMember, ResolutionContext context)
        {
            _context = context;
            return new SvcModels.GeoCoordinate(){Lon = sourceMember.X, Lat = sourceMember.Y};
        }
    }
}