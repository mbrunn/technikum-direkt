using System.Linq;
using Microsoft.EntityFrameworkCore;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Sql
{
    public class ParcelRepository : IParcelRepository
    {
        private readonly TechnikumDirektContext _dbContext;

        public ParcelRepository(TechnikumDirektContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public Parcel GetByTrackingId(string trackingId)
        {
            return _dbContext.Parcels
                .Include(p => p.HopArrivals)
                .First(p => p.TrackingId == trackingId);
        }

        public void Update(Parcel parcel)
        {
            _dbContext.Parcels.Update(parcel);
            _dbContext.SaveChanges();
        }

        public string Add(Parcel parcel)
        {
            _dbContext.Parcels.Add(parcel);

            _dbContext.HopArrivals.AddRange(parcel.HopArrivals);
            
            _dbContext.SaveChanges();
            
            return parcel.TrackingId;
        }

        public void Delete(Parcel parcel)
        {
            _dbContext.Parcels.Remove(parcel);
            _dbContext.SaveChanges();
        }

        public void Delete(string trackingId)
        {
            var parcel = GetByTrackingId(trackingId);
            _dbContext.Remove(parcel);
            _dbContext.SaveChanges();
        }
    }
}