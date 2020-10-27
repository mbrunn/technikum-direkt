using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.BusinessLogic.Models;
using TechnikumDirekt.DataAccess.Interfaces;
using HopArrival = TechnikumDirekt.BusinessLogic.Models.HopArrival;
using Parcel = TechnikumDirekt.BusinessLogic.Models.Parcel;
using Recipient = TechnikumDirekt.BusinessLogic.Models.Recipient;

namespace TechnikumDirekt.BusinessLogic
{
    public class TrackingLogic: ITrackingLogic
    {
        private readonly IValidator<Parcel> _parcelValidator;
        private readonly IValidator<Recipient> _recipientValidator;
        private readonly IValidator<HopArrival> _hopArrivalValidator;
        private readonly IValidator<Hop> _hopCodeValidator;

        private readonly IHopRepository _hopRepository;
        public TrackingLogic(IValidator<Parcel> parcelValidator, IValidator<Recipient> recipientValidator, IValidator<HopArrival> hopArrivalValidator, 
            IValidator <Hop> hopCodeValidator, IHopRepository hopRepository)
        {
            _parcelValidator = parcelValidator;
            _recipientValidator = recipientValidator;
            _hopArrivalValidator = hopArrivalValidator;
            _hopCodeValidator = hopCodeValidator;
            _hopRepository = hopRepository;
        }
        
        private const int IdLength = 9;
        
        private readonly List<Parcel> _parcels = new List<Parcel>
        {
            new Parcel { TrackingId = "ABCD12345", State = Parcel.StateEnum.InTruckDeliveryEnum}
        };
        
        public void ReportParcelDelivery(string trackingId)
        {
            _parcelValidator.Validate(new Parcel {TrackingId = trackingId},
                options =>
                {
                    options.IncludeRuleSets("trackingId");
                    options.ThrowOnFailures();
                });
            
            var parcel = _parcels.Find(p => p.TrackingId == trackingId);
            
            if (parcel == null) throw new TrackingLogicException($"Parcel for tracking id {trackingId} not found"); // TODO - use notfound exception

            parcel.State = Parcel.StateEnum.DeliveredEnum;
        }

        public void ReportParcelHop(string trackingId, string code)
        {
            _hopCodeValidator.Validate(new Hop {Code = code}, 
                options =>
                {
                    options.IncludeRuleSets("code");
                    options.ThrowOnFailures();
                });
            
            _parcelValidator.Validate(new Parcel {TrackingId = trackingId},
                options =>
                {
                    options.IncludeRuleSets("trackingId");
                    options.ThrowOnFailures();
                });
        
            var parcel = _parcels.Find(p => p.TrackingId == trackingId);
            var hop = _hopRepository.GetHopByCode(code);
            
            if (parcel == null) throw new TrackingLogicException($"Parcel for tracking id {trackingId} not found");
            if (hop == null) throw new TrackingLogicException($"Warehouse for code {code} not found");
            
            parcel.VisitedHops.Add(new HopArrival
            {
                Code = code,
                HopArrivalTime = DateTime.Now
            });
        }

        //TODO: return Parcel to extract Tracking Info ?
        public string SubmitParcel(Parcel parcel)
        {
            ValidateParcel(parcel);
            do
            {
                parcel.TrackingId = GenerateUniqueId(IdLength);
            } while (_parcels.Find(x => x.TrackingId == parcel.TrackingId) != null);
            
            _parcels.Add(parcel);
            return parcel.TrackingId;
        }

        public Parcel TrackParcel(string trackingId)
        {
            _parcelValidator.Validate(new Parcel {TrackingId = trackingId},
                options =>
                {
                    options.IncludeRuleSets("trackingId");
                    options.ThrowOnFailures();
                });
            
            var parcel = _parcels.Find(p => p.TrackingId == trackingId);
            
            if (parcel == null) throw new TrackingLogicException($"Parcel for tracking id {trackingId} not found");

            return parcel;
        }

        public void TransitionParcelFromPartner(Parcel parcel, string trackingId)
        {
            _parcelValidator.Validate(new Parcel {TrackingId = trackingId},
                options =>
                {
                    options.IncludeRuleSets("trackingId");
                    options.ThrowOnFailures();
                });
            _parcelValidator.ValidateAndThrow(parcel);
            
            if (_parcels.Find(p => p.TrackingId == trackingId) != null) throw new TrackingLogicException($"A parcel with tracking id {trackingId} has already been registered");
            
            parcel.TrackingId = trackingId;

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

        private void ValidateParcel(Parcel parcel)
        {
            _parcelValidator.ValidateAndThrow(parcel);
            _recipientValidator.ValidateAndThrow(parcel.Recipient);
            _recipientValidator.ValidateAndThrow(parcel.Sender);

            foreach (var futureHop in parcel.FutureHops)
            {
                _hopArrivalValidator.ValidateAndThrow(futureHop);
            }
            
            foreach (var visitedHop in parcel.VisitedHops)
            {
                _hopArrivalValidator.ValidateAndThrow(visitedHop);
            }
        }
    }
}