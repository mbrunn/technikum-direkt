using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;
using TechnikumDirekt.DataAccess.Sql.Exceptions;

namespace TechnikumDirekt.DataAccess.Sql
{
    public class HopRepository : IHopRepository
    {
        private readonly ITechnikumDirektContext _dbContext;
        private readonly ILogger<HopRepository> _logger;

        public HopRepository(ITechnikumDirektContext dbContext, ILogger<HopRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Hop GetHopByCode(string hopCode)
        {
            if (string.IsNullOrEmpty(hopCode)) throw new DataAccessArgumentNullException("HopCode is null.");
            
            var hop = _dbContext.Hops.Find(hopCode);
            if (hop != null)
            {
                _logger.LogTrace($"Hop with code {hopCode} has been found.");
            }
            else
            {
                _logger.LogTrace($"Hop with code {hopCode} couldn't be found.");
                throw new DataAccessNotFoundException($"Hop with code {hopCode} couldn't be found.");
            }

            return hop;
        }

        public Hop GetHopContainingPoint(Point point)
        {
            if(point == null) throw new DataAccessArgumentNullException("Point is null.");
            
            var truck = _dbContext.Trucks.FirstOrDefault(t => t.RegionGeometry.Contains(point));
            
            if (truck != null)
            {
                _logger.LogTrace($"Truck containing Point {point.Coordinate} has been found.");
                return truck;
            }

            var transferwarehouse = _dbContext.Transferwarehouses.FirstOrDefault(t => t.RegionGeometry.Contains(point));
            
            if (transferwarehouse != null)
            {
                _logger.LogTrace($"Transferwarehouse containing Point {point.Coordinate} has been found.");
                return transferwarehouse;
            }

            _logger.LogTrace($"Hop containing the point {point.Coordinate} couldn't be found.");
            throw new DataAccessNotFoundException($"Hop containing the point {point.Coordinate} couldn't be found.");
        }
    }
}