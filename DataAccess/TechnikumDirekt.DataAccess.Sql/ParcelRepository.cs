using System;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;
using TechnikumDirekt.DataAccess.Sql.Exceptions;

namespace TechnikumDirekt.DataAccess.Sql
{
    public class ParcelRepository : IParcelRepository
    {
        private readonly ITechnikumDirektContext _dbContext;
        private readonly ILogger<ParcelRepository> _logger;

        public ParcelRepository(ITechnikumDirektContext dbContext, ILogger<ParcelRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Parcel GetByTrackingId(string trackingId)
        {
            if (string.IsNullOrEmpty(trackingId)) throw new DataAccessArgumentNullException("TrackingId is null.");
            
            var parcel = _dbContext.Parcels
                .Include(p => p.HopArrivals)
                .ThenInclude(p => p.Hop)
                .FirstOrDefault(p => p.TrackingId == trackingId);

            if (parcel != null)
            {
                _logger.LogTrace($"Parcel with trackingId {trackingId} has been found.");
            }
            else
            {
                _logger.LogTrace($"Parcel with trackingId {trackingId} couldn't be found.");
                throw new DataAccessNotFoundException($"Parcel with trackingId {trackingId} couldn't be found.");
            }

            return parcel;
        }

        public void Update(Parcel parcel)
        {
            _dbContext.Parcels.Update(parcel);
            _dbContext.SaveChanges();
            _logger.LogTrace($"Parcel with trackindId {parcel.TrackingId} has been updated.");
        }

        public string Add(Parcel parcel)
        {
            _dbContext.Parcels.Add(parcel);

            int i = 0;
            
            foreach (var ha in parcel.HopArrivals)
            {
                ha.Order = i;
                i++;
            }
            
            _dbContext.HopArrivals.AddRange(parcel.HopArrivals);

            _dbContext.SaveChanges();
            _logger.LogTrace($"Parcel with trackindId {parcel.TrackingId} has been added.");

            return parcel.TrackingId;
        }

        public void Delete(Parcel parcel)
        {
            _dbContext.Parcels.Remove(parcel);
            _dbContext.SaveChanges();
            _logger.LogTrace($"Parcel with trackindId {parcel.TrackingId} has been deleted by parcel object.");
        }

        public void Delete(string trackingId)
        {
            var parcel = GetByTrackingId(trackingId);
            _dbContext.Remove(parcel);
            _dbContext.SaveChanges();
            _logger.LogTrace($"Parcel with trackindId {trackingId} has been deleted by trackingId");
        }
    }
}