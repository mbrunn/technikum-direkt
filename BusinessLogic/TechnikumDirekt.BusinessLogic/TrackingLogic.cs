using System;
using System.Linq;
using System.Text;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.BusinessLogic.Models;
using TechnikumDirekt.DataAccess.Interfaces;
using DalModels = TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.BusinessLogic
{
    public class TrackingLogic : ITrackingLogic
    {
        private readonly IValidator<Parcel> _parcelValidator;
        private readonly IValidator<Recipient> _recipientValidator;
        private readonly IValidator<HopArrival> _hopArrivalValidator;
        private readonly IValidator<Hop> _hopCodeValidator;

        private readonly IMapper _mapper;

        private readonly IHopRepository _hopRepository;
        private readonly IParcelRepository _parcelRepository;

        private ILogger _logger;

        public TrackingLogic(IValidator<Parcel> parcelValidator, IValidator<Recipient> recipientValidator,
            IValidator<HopArrival> hopArrivalValidator,
            IValidator<Hop> hopCodeValidator, IHopRepository hopRepository, IParcelRepository parcelRepository,
            IMapper mapper, ILogger<TrackingLogic> logger)
        {
            _parcelValidator = parcelValidator;
            _recipientValidator = recipientValidator;
            _hopArrivalValidator = hopArrivalValidator;
            _hopCodeValidator = hopCodeValidator;
            _hopRepository = hopRepository;
            _parcelRepository = parcelRepository;
            _mapper = mapper;
            _logger = logger;
        }

        private const int IdLength = 9;

        public void ReportParcelDelivery(string trackingId)
        {
            _parcelValidator.Validate(new Parcel {TrackingId = trackingId},
                options =>
                {
                    options.IncludeRuleSets("trackingId");
                    options.ThrowOnFailures();
                });

            var parcel = _parcelRepository.GetByTrackingId(trackingId);

            if (parcel == null)
            {
                _logger.LogTrace($"Parcel for TrackingId {trackingId} not found");
                throw new TrackingLogicException($"Parcel for TrackingId {trackingId} not found");
            }

            //TODO - Check if Parcel is already delivered ?
            parcel.State = DalModels.Parcel.StateEnum.DeliveredEnum;
            _parcelRepository.Update(parcel);
            _logger.LogDebug($"Parcel with TrackingId {trackingId} has been set to delivered.");
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

            var parcel = _parcelRepository.GetByTrackingId(trackingId);
            var hop = _hopRepository.GetHopByCode(code);

            if (parcel == null)
            {
                _logger.LogTrace($"Parcel with TrackingId {trackingId} couldn't be found.");
                throw new TrackingLogicException($"Parcel for tracking id {trackingId} not found");
            }

            if (hop == null)
            {
                _logger.LogTrace($"Hop with hopCode {code} couldn't be found.");
                throw new TrackingLogicException($"Warehouse for code {code} not found");
            }

            var hopToEdit = parcel.HopArrivals.Find(x => x.HopCode == code);

            if (hopToEdit == null)
            {
                _logger.LogTrace($"Hop to edit with hopCode {code} couldn't be found.");
                _logger.LogTrace($"Parcel with TrackingId {trackingId} has been set to delivered.");
                throw new TrackingLogicException($"Hop with code {code} is not part of this parcel's route.");
            }

            parcel.HopArrivals.Remove(hopToEdit);

            hopToEdit.HopArrivalTime = DateTime.Now;

            parcel.HopArrivals.Add(hopToEdit);

            _parcelRepository.Update(parcel);
            _logger.LogDebug($"Hop with hopCode {code} couldn't be found.");
        }

        public string SubmitParcel(Parcel parcel)
        {
            ValidateParcel(parcel);

            #region TestDataInsertion

            /*parcel.VisitedHops = new List<HopArrival>()
            {
                new HopArrival()
                {
                    HopArrivalTime = DateTime.Now,
                    Code = "WSTB02",
                    Description = "Warehouse Level 2 - Wien Stadt"
                }
            };
            
            parcel.FutureHops = new List<HopArrival>()
            {
                new HopArrival()
                {
                    HopArrivalTime = null,    
                    Code = "WENB01",
                    Description = "Warehouse Level 1 - Wien"
                },
                new HopArrival()
                {
                    HopArrivalTime = null,
                    Code = "WTTA087",
                    Description = "Truck in Süssenbrunn"
                }
            };*/

            #endregion

            while (true)
            {
                try
                {
                    parcel.TrackingId = GenerateUniqueId(IdLength);
                    _parcelRepository.Add(_mapper.Map<DalModels.Parcel>(parcel));
                    _logger.LogDebug($"Parcel with TrackingId {parcel.TrackingId} has been added.");
                    return parcel.TrackingId;
                }
                catch (DbUpdateException)
                {
                    //this exception is thrown if trackingId already exists.
                    _logger.LogTrace($"TrackingId {parcel.TrackingId} already exists.");
                }
            }
        }

        public Parcel TrackParcel(string trackingId)
        {
            _parcelValidator.Validate(new Parcel {TrackingId = trackingId},
                options =>
                {
                    options.IncludeRuleSets("trackingId");
                    options.ThrowOnFailures();
                });

            var parcel = _mapper.Map<Parcel>(_parcelRepository.GetByTrackingId(trackingId));

            if (parcel == null)
            {
                _logger.LogTrace($"Parcel with trackingId {trackingId} not found");
                throw new TrackingLogicException($"Parcel for tracking id {trackingId} not found");
            }

            _logger.LogDebug($"Parcel with trackingId {trackingId} is being tracked.");
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

            if (_parcelRepository.GetByTrackingId(trackingId) != null)
            {
                _logger.LogTrace($"A parcel with tracking id {trackingId} has already been registered");
                throw new TrackingLogicException($"A parcel with tracking id {trackingId} has already been registered");
            }

            parcel.TrackingId = trackingId;
            _parcelRepository.Add(_mapper.Map<DalModels.Parcel>(parcel));
            _logger.LogDebug($"Parcel with trackingId {trackingId} has been transitioned from partner");
        }

        //Generator to generate Base 32 based ID with built in GUID as random generator
        private static string GenerateUniqueId(int length)
        {
            var builder = new StringBuilder();
            Enumerable
                .Range(65, 26)
                .Select(e => ((char) e).ToString())
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