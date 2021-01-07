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
                .Include(p => p.Recipient)
                .Include(p => p.Sender)
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
            try
            {
                _dbContext.Parcels.Update(parcel);
                _dbContext.SaveChanges();
                _logger.LogTrace($"Parcel with trackindId {parcel.TrackingId} has been updated.");
            }
            catch (Exception e)
            {
                _logger.LogTrace($"Parcel with TrackingId {parcel.TrackingId} could not be updated.");
                throw new DataAccessUpdateException();
            }
        }

        public string Add(Parcel parcel)
        {
            try
            {
                _dbContext.Parcels.Add(parcel);
                _dbContext.HopArrivals.AddRange(parcel.HopArrivals);

                _dbContext.SaveChanges();
                _logger.LogTrace($"Parcel with trackindId {parcel.TrackingId} has been added.");

                return parcel.TrackingId;
            }
            catch (Exception e)
            {
                _logger.LogTrace($"Parcel with TrackingId {parcel.TrackingId} could not be added.");
                throw new DataAccessAddException();
            }
        }

        /// <summary>
        /// Deletes the given parcel in the database
        /// </summary>
        /// <param name="parcel">
        /// Parcel that should be deleted
        /// </param>
        public void Delete(Parcel parcel)
        {
            try
            {
                _dbContext.Parcels.Remove(parcel);
                _dbContext.SaveChanges();
                _logger.LogTrace($"Parcel with trackindId {parcel.TrackingId} has been deleted by parcel object.");
            }
            catch (Exception e)
            {
                _logger.LogTrace($"Parcel with TrackingId {parcel.TrackingId} could not be deleted.");
                throw new DataAccessRemoveException();
            }
        }

        /// <summary>
        /// Deletes the parcel with the given TrackingId in the database
        /// </summary>
        /// <param name="parcel">
        /// TrackingId of the Parcel that should be deleted
        /// </param>
        public void Delete(string trackingId)
        {
            try
            {
                var parcel = GetByTrackingId(trackingId);
                _dbContext.Remove(parcel);
                _dbContext.SaveChanges();
                _logger.LogTrace($"Parcel with trackindId {trackingId} has been deleted by trackingId");
            }
            catch (Exception e)
            {
                _logger.LogTrace($"Parcel with TrackingId {trackingId} could not be deleted.");
                throw new DataAccessRemoveException();
            }
        }

        /// <summary>
        /// Checks if Recipient exists in Database.
        /// </summary>
        /// <param name="recipient">
        /// The recipient to check for. (All properties except Id are checked for)
        /// </param>
        /// <returns>
        ///    Id if found, null if no Recipient exists with given data.
        /// </returns>
        private int? GetRecipientId(Recipient recipient)
        {
            try
            {
                return _dbContext.Recipients.FirstOrDefault(sender =>
                    sender.City == recipient.City &&
                    sender.Country == recipient.Country
                    && sender.Name == recipient.Name &&
                    sender.Street == recipient.Street &&
                    sender.PostalCode == recipient.PostalCode)?.Id;
            }
            catch (Exception e)
            {
                _logger.LogTrace($"Recipient with Name {recipient.Name} could not be found.");
                throw new DataAccessNotFoundException();
            }
        }
    }
}