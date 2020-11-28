using System.Data;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NetTopologySuite.Geometries;
using SvcModels = TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Services.Mapper
{
    public class PointConverter : IValueConverter<SvcModels.GeoCoordinate, Point>,
        IValueConverter<Point, SvcModels.GeoCoordinate>
    {
        private ResolutionContext _context;
        private readonly ILogger<PointConverter> _logger;

        public PointConverter(ILogger<PointConverter> logger)
        {
            _logger = logger;
        }
        
        public PointConverter()
        {
            _logger = NullLogger<PointConverter>.Instance;
        }

        /// <summary>
        /// Defines Converters for converting SvcModels.GeoCoordinate 
        /// </summary>
        /// <param name="sourceMember"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NoNullAllowedException"></exception>
        Point IValueConverter<SvcModels.GeoCoordinate, Point>.Convert(SvcModels.GeoCoordinate sourceMember,
            ResolutionContext context)
        {
            _context = context;
            if (sourceMember.Lat == null || sourceMember.Lon == null)
            {
                _logger.LogDebug("");
                throw new NoNullAllowedException();
            }

            return new Point((double) sourceMember.Lon, (double) sourceMember.Lat) {SRID = 4326};
        }

        public SvcModels.GeoCoordinate Convert(Point sourceMember, ResolutionContext context)
        {
            _context = context;
            return new SvcModels.GeoCoordinate() {Lon = sourceMember.X, Lat = sourceMember.Y};
        }
    }
}