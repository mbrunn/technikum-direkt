using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;

using HopArrival = TechnikumDirekt.BusinessLogic.Models.HopArrival;
using Parcel = TechnikumDirekt.BusinessLogic.Models.Parcel;

namespace TechnikumDirekt.BusinessLogic
{
    public class TrackingLogic: ITrackingLogic
    {
        private readonly IValidator<Parcel> _parcelValidator;
        public TrackingLogic(IValidator<Parcel> parcelValidator)
        {
            _parcelValidator = parcelValidator;
        }
        
        private const int IdLength = 9;
        
        private readonly List<Parcel> _parcels = new List<Parcel>
        {
            new Parcel { TrackingId = "ABCD12345", State = Parcel.StateEnum.InTruckDeliveryEnum}
        };
        
        public void ReportParcelDelivery(string trackingId)
        {
            var parcel = _parcels.Find(p => p.TrackingId == trackingId);
            
            if (parcel == null) throw new TrackingLogicException($"Parcel for tracking id {trackingId} not found");

            parcel.State = Parcel.StateEnum.DeliveredEnum;
        }

        public void ReportParcelHop(string trackingId, string code)
        {
            var parcel = _parcels.Find(p => p.TrackingId == trackingId);
            var warehouse = WarehouseLogic.Warehouses.Find(w => w.Code == code);
            
            if (parcel == null) throw new TrackingLogicException($"Parcel for tracking id {trackingId} not found");
            if (warehouse == null) throw new TrackingLogicException($"Warehouse for code {code} not found");
            
            parcel.VisitedHops.Add(new HopArrival
            {
                Code = code,
                DateTime = DateTime.Now
            });
        }

        public void SubmitParcel(Parcel parcel)
        {
            //should we try/catch and throw a TrackingLogicException here too ?
            do
            {
                parcel.TrackingId = GenerateUniqueId(IdLength);
            } while (_parcels.Find(x => x.TrackingId == parcel.TrackingId) != null);

            _parcelValidator.ValidateAndThrow(parcel);
            
            _parcels.Add(parcel);
        }

        public Parcel TrackParcel(string trackingId)
        {
            var parcel = _parcels.Find(p => p.TrackingId == trackingId);
            
            if (parcel == null) throw new TrackingLogicException($"Parcel for tracking id {trackingId} not found");

            return parcel;
        }

        public void TransitionParcelFromPartner(Parcel parcel, string trackingId)
        {
            //shouldn't we generate a unique ID if the ID already is present in our system ?
            do
            {
                parcel.TrackingId = GenerateUniqueId(IdLength);
            } while (_parcels.Find(x => x.TrackingId == parcel.TrackingId) != null);
            
            //if (_parcels.Find(p => p.TrackingId == trackingId) != null) throw new TrackingLogicException($"A parcel with tracking id {trackingId} has already been registered");
            
            parcel.TrackingId = trackingId;

            _parcelValidator.ValidateAndThrow(parcel);
            
            _parcels.Add(parcel);
        }
        
        //Generator to generate Base 32 based ID with built in GUID as random generator
        private static string GenerateUniqueId(int length)
        {
            var builder = new StringBuilder();
            Enumerable
                .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(length)
                .ToList().ForEach(e => builder.Append(e));

            return builder.ToString();
        }
    }
}