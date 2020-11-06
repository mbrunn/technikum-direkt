using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;

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
            var parcel = _dbContext.Parcels
                .Include(p => p.HopArrivals)
                .FirstOrDefault(p => p.TrackingId == trackingId);
            
            if (parcel != null)
            {
                _logger.LogTrace($"Parcel with trackingId {trackingId} has been found.");
            }
            else
            {
                _logger.LogTrace($"Parcel with trackingId {trackingId} couldn't be found.");
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