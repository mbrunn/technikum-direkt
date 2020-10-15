using System;
using System.Collections.Generic;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic
{
    public class TrackingLogic: ITrackingLogic
    {
        private readonly List<Parcel> _parcels = new List<Parcel>
        {
            new Parcel { TrackingId = "abcd1234", State = Parcel.StateEnum.InTruckDeliveryEnum }
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
            if (_parcels.Find(p => p.TrackingId == trackingId) != null) throw new TrackingLogicException($"A parcel with tracking id {trackingId} has already been registered");
            
            parcel.TrackingId = trackingId;
            _parcels.Add(parcel);
        }
    }
}