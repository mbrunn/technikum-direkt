using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.BusinessLogic.Models;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Sql.Exceptions;
using TechnikumDirekt.ServiceAgents.Exceptions;
using TechnikumDirekt.ServiceAgents.Interfaces;
using TechnikumDirekt.ServiceAgents.Models;
using DalModels = TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.BusinessLogic
{
    public class TrackingLogic : ITrackingLogic
    {
        private readonly IValidator<Parcel> _parcelValidator;
        private readonly IValidator<Recipient> _recipientValidator;
        private readonly IValidator<HopArrival> _hopArrivalValidator;
        private readonly IValidator<Hop> _hopCodeValidator;

        private readonly IHopRepository _hopRepository;
        private readonly IParcelRepository _parcelRepository;
        private readonly IWarehouseLogic _warehouseLogic;

        private readonly IGeoEncodingAgent _geoEncodingAgent;
        private readonly ILogisticsPartnerAgent _logisticsPartnerAgent;
        
        private readonly IMapper _mapper;

        private readonly ILogger _logger;

        public TrackingLogic(IValidator<Parcel> parcelValidator, IValidator<Recipient> recipientValidator,
            IValidator<HopArrival> hopArrivalValidator,
            IValidator<Hop> hopCodeValidator, IHopRepository hopRepository, IParcelRepository parcelRepository,
            IGeoEncodingAgent geoEncodingAgent, ILogisticsPartnerAgent logisticsPartnerAgent, IWarehouseLogic warehouseLogic, IMapper mapper, 
            ILogger<TrackingLogic> logger)
        {
            _parcelValidator = parcelValidator;
            _recipientValidator = recipientValidator;
            _hopArrivalValidator = hopArrivalValidator;
            _hopCodeValidator = hopCodeValidator;
            _hopRepository = hopRepository;
            _parcelRepository = parcelRepository;
            _geoEncodingAgent = geoEncodingAgent;
            _logisticsPartnerAgent = logisticsPartnerAgent;
            _warehouseLogic = warehouseLogic;
            _mapper = mapper;
            _logger = logger;
        }

        private const int IdLength = 9;

        public void ReportParcelDelivery(string trackingId)
        {
            try
            {
                _parcelValidator.Validate(new Parcel {TrackingId = trackingId},
                    options =>
                    {
                        options.IncludeRuleSets("trackingId");
                        options.ThrowOnFailures();
                    });
            }
            catch (ValidationException e)
            {
                throw new BusinessLogicValidationException("Parcel validation failed.", e);
            }

            try
            {
                var parcel = _parcelRepository.GetByTrackingId(trackingId);
                parcel.State = DalModels.Parcel.StateEnum.DeliveredEnum;
                parcel.HopArrivals.OrderBy(ha => ha.Order).Last().HopArrivalTime = DateTime.Now;
                _parcelRepository.Update(parcel);
            }
            catch (DataAccessNotFoundException e)
            {
                _logger.LogTrace($"Parcel for TrackingId {trackingId} not found");
                throw new BusinessLogicNotFoundException($"Parcel for TrackingId {trackingId} not found", e);
            }

            //TODO - Check if Parcel is already delivered ?
            _logger.LogDebug($"Parcel with TrackingId {trackingId} has been set to delivered.");
        }

        public void ReportParcelHop(string trackingId, string code)
        {
            try
            {
                _hopCodeValidator.Validate(new Hop {Code = code},
                    options =>
                    {
                        options.IncludeRuleSets("code");
                        options.ThrowOnFailures();
                    });
            }
            catch (ValidationException e)
            {
                throw new BusinessLogicValidationException("Hop code validation failed.", e);
            }
            
            try
            {
                _parcelValidator.Validate(new Parcel {TrackingId = trackingId},
                    options =>
                    {
                        options.IncludeRuleSets("trackingId");
                        options.ThrowOnFailures();
                    });
            }
            catch (ValidationException e)
            {
                throw new BusinessLogicValidationException("Parcel validation failed.", e);
            }

            DalModels.Parcel parcel = null;
            
            try
            {
                parcel = _parcelRepository.GetByTrackingId(trackingId);
            }
            catch (DataAccessNotFoundException e)
            {
                _logger.LogTrace($"Parcel for TrackingId {trackingId} not found");
                throw new BusinessLogicNotFoundException($"Parcel for TrackingId {trackingId} not found", e);
            }

            try
            {
                _hopRepository.GetHopByCode(code);
            }
            catch (DataAccessNotFoundException e)
            {
                _logger.LogTrace($"Hop with hopCode {code} couldn't be found.");
                throw new BusinessLogicNotFoundException($"Warehouse for code {code} not found", e);
            }

            var hopToEdit = parcel.HopArrivals.Find(x => x.HopCode == code);

            if (hopToEdit == null)
            {
                _logger.LogTrace($"Hop to edit with hopCode {code} couldn't be found.");
                _logger.LogTrace($"Parcel with TrackingId {trackingId} has been set to delivered.");
                throw new BusinessLogicBadArgumentException($"Hop with code {code} is not part of this parcel's route.");
            }
            
            /*
            if (hopToEdit.HopCode == "WTTA014")
            {
                var hop = (DalModels.Truck) hopToEdit.Hop;
                hopToEdit.Hop = new DalModels.Transferwarehouse()
                {
                    Code = hopToEdit.HopCode,
                    Description = "Transferwarehouse in Testcity",
                    HopArrivals = new List<DalModels.HopArrival>(),
                    HopType = DalModels.HopType.TransferWarehouse,
                    LocationCoordinates = hop.LocationCoordinates,
                    LocationName = "AUSLAND",
                    LogisticsPartner = "Yeetmann Gruppe",
                    LogisticsPartnerUrl = "https://technikumdirektapi.azurewebsites.net/"
                };
            }*/

            if (hopToEdit.Hop.HopType == DalModels.HopType.TransferWarehouse)
            {
                var blParcel = _mapper.Map<Parcel>(parcel);
                var svcParcel = _mapper.Map<Services.Models.Parcel>(blParcel);
            
                var blHop = _mapper.Map<Transferwarehouse>(hopToEdit.Hop);
                var svcHop = _mapper.Map<Services.Models.Transferwarehouse>(blHop);

                try
                {
                    _logisticsPartnerAgent.TransitionParcelToPartner(parcel.TrackingId, svcParcel, svcHop);
                }
                catch (ServiceAgentsBadResponseException e)
                {
                    _logger.LogTrace($"Transferwarehouse {svcHop.Description} is not responding or has invalid URL.");
                    throw new BusinessLogicBadArgumentException(
                        $"Transferwarehouse {svcHop.Description} is not responding or has invalid URL.");
                }
            }

            switch (hopToEdit.Hop.HopType)
            {
                case DalModels.HopType.Warehouse:
                    parcel.State = DalModels.Parcel.StateEnum.InTransportEnum;
                    hopToEdit.HopArrivalTime = DateTime.Now;
                    break;
                case DalModels.HopType.Truck:
                    if (parcel.HopArrivals.Count(ha => ha.HopArrivalTime != null) > 1)
                    {
                        parcel.State = DalModels.Parcel.StateEnum.InTruckDeliveryEnum;
                        hopToEdit.HopArrivalTime = DateTime.Now;
                    }
                    break;
                case DalModels.HopType.TransferWarehouse:
                    parcel.State = DalModels.Parcel.StateEnum.TransferredEnum;
                    hopToEdit.HopArrivalTime = DateTime.Now;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            parcel.HopArrivals.Remove(hopToEdit);
            parcel.HopArrivals.Add(hopToEdit);

            _parcelRepository.Update(parcel);
            
            _logger.LogDebug($"Parcel with trackingId {trackingId} updated.");
        }

        public string SubmitParcel(Parcel parcel)
        {
            try
            {
                ValidateParcel(parcel);
            }
            catch (ValidationException e)
            {
                throw new BusinessLogicValidationException("Parcel validation failed.", e);
            }
            
            parcel = FindShortestPath(parcel);
            
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
            try
            {
                _parcelValidator.Validate(new Parcel {TrackingId = trackingId},
                    options =>
                    {
                        options.IncludeRuleSets("trackingId");
                        options.ThrowOnFailures();
                    });
            }
            catch (ValidationException e)
            {
                throw new BusinessLogicValidationException("Parcel validation failed.", e);
            }

            DalModels.Parcel dalParcel;
            
            try
            {
                dalParcel = _parcelRepository.GetByTrackingId(trackingId);
            }
            catch (DataAccessNotFoundException e)
            {
                _logger.LogTrace($"Parcel for TrackingId {trackingId} not found");
                throw new BusinessLogicNotFoundException($"Parcel for TrackingId {trackingId} not found", e);
            }
            
            var parcel = _mapper.Map<Parcel>(dalParcel);
            
            _logger.LogDebug($"Parcel with trackingId {trackingId} is being tracked.");
            return parcel;
        }

        public void TransitionParcelFromPartner(Parcel parcel, string trackingId)
        {
            try
            {
                _parcelValidator.Validate(new Parcel {TrackingId = trackingId},
                    options =>
                    {
                        options.IncludeRuleSets("trackingId");
                        options.ThrowOnFailures();
                    });
                _parcelValidator.ValidateAndThrow(parcel);
            }
            catch (ValidationException e)
            {
                throw new BusinessLogicValidationException("Parcel validation failed.", e);
            }

            try
            {
                _parcelRepository.GetByTrackingId(trackingId);
                _logger.LogTrace($"A parcel with tracking id {trackingId} has already been registered");
                throw new BusinessLogicBadArgumentException($"A parcel with tracking id {trackingId} has already been registered");
            }
            catch (DataAccessNotFoundException)
            {
                parcel.TrackingId = trackingId;
                _parcelRepository.Add(_mapper.Map<DalModels.Parcel>(parcel));
                _logger.LogDebug($"Parcel with trackingId {trackingId} has been transitioned from partner");
            }
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

        private Parcel FindShortestPath(Parcel parcel)
        {
            var futureHops = new List<HopArrival>();
            
            //1. get Coordinates of Sender and Recipient:
            if (parcel.Sender == null)
            {
                throw new BusinessLogicBadArgumentException($"Sender can not be null.");
            }
            
            if (parcel.Recipient == null)
            {
                throw new BusinessLogicBadArgumentException($"Recipient can not be null.");
            }
            
            var senderAddress = new Address()
            {
                City = parcel.Sender.City,
                Country = parcel.Sender.Country,
                PostalCode = parcel.Sender.PostalCode,
                Street = parcel.Sender.Street
            };
            
            var recipientAddress = new Address()
            {
                City = parcel.Recipient.City,
                Country = parcel.Recipient.Country,
                PostalCode = parcel.Recipient.PostalCode,
                Street = parcel.Recipient.Street
            };
            
            Point senderPoint;
            
            try
            {
                senderPoint = _geoEncodingAgent.EncodeAddress(senderAddress);
            }
            catch (ServiceAgentsNotFoundException e)
            {
                throw new BusinessLogicFutureHopsPrediction($"Address of Sender {parcel.Sender.Name} could not be decoded into GPS Coordinates.");
            }
            
            Point recipientPoint;

            try
            {
                recipientPoint = _geoEncodingAgent.EncodeAddress(recipientAddress);
            }
            catch (ServiceAgentsNotFoundException e)
            {
                throw new BusinessLogicFutureHopsPrediction($"Address of Sender {parcel.Recipient.Name} could not be decoded into GPS Coordinates.");
            }


            //2. get nearest Truck/Hop to Sender and Recipient:
            
            Hop nearestHoptoSender;
            
            try
            { 
                nearestHoptoSender = _warehouseLogic.GetHopContainingPoint(senderPoint);
            }
            catch (DataAccessArgumentNullException e)
            {
                throw new BusinessLogicFutureHopsPrediction($"No Hop contains the given Point {senderPoint.AsText()} could not be decoded into GPS Coordinates.");
            }

            Hop nearestHoptoReceipient;
            
            try
            { 
                nearestHoptoReceipient = _warehouseLogic.GetHopContainingPoint(recipientPoint);
            }
            catch (DataAccessArgumentNullException e)
            {
                throw new BusinessLogicFutureHopsPrediction($"No Hop contains the given Point {recipientPoint.AsText()} could not be decoded into GPS Coordinates.");
            }
            
            //3. find shortest path.
            parcel.FutureHops = FindFutureHops(nearestHoptoSender, nearestHoptoReceipient);
            
            parcel.VisitedHops = new List<HopArrival>()
            {
                new HopArrival()
                {
                    Code = nearestHoptoSender.Code,
                    Description = nearestHoptoSender.Description,
                    HopArrivalTime = DateTime.Now
                }
            };

            parcel.State = Parcel.StateEnum.PickupEnum;
            
            return parcel;
        }

        private List<HopArrival> FindFutureHops(Hop senderHop, Hop recipientHop)
        {
            var senderHopArrivals = new List<HopArrival>();
            var recipientHopArrivals = new List<HopArrival>();

            DalModels.Hop currentSenderHop;
            DalModels.Hop currentRecipientHop;

            try
            {
                currentSenderHop = _hopRepository.GetHopByCode(senderHop.Code);
            }
            catch (DataAccessNotFoundException)
            {
                throw new BusinessLogicFutureHopsPrediction($"Nearest Hop to Sender with code {senderHop.Code} does not exist. FutureHops Prediction failed.");
            }
            
            try
            {
                currentRecipientHop = _hopRepository.GetHopByCode(recipientHop.Code);
            }
            catch (DataAccessNotFoundException)
            {
                throw new BusinessLogicFutureHopsPrediction($"Nearest Hop to Recipient with code {recipientHop.Code} does not exist. FutureHops Prediction failed.");
            }
            
            var senderWhCode = currentSenderHop.ParentWarehouseCode;
            var recipientWhCode = currentRecipientHop.ParentWarehouseCode;

            #region BringToSameLevel

            var senderWh = new DalModels.Warehouse();
            var recipientWh = new DalModels.Warehouse();
            
            try
            {
                senderWh = (DalModels.Warehouse) _hopRepository.GetHopByCode(senderWhCode);
            }
            catch (DataAccessNotFoundException)
            {
                throw new BusinessLogicFutureHopsPrediction($"Warehouse with code {senderWhCode} does not exist. FutureHops Prediction failed.");
            }
            
            try
            {
                recipientWh = (DalModels.Warehouse) _hopRepository.GetHopByCode(recipientWhCode);
            }
            catch (DataAccessNotFoundException)
            {
                throw new BusinessLogicFutureHopsPrediction($"Warehouse with code {recipientWhCode} does not exist. FutureHops Prediction failed.");
            }
            
            if (senderWh.Level != recipientWh.Level)
            {
                //not on matching levels:
                if (currentRecipientHop.ParentWarehouse.Level < currentSenderHop.ParentWarehouse.Level)
                {
                    //recipient is lower down the tree -> go the same level as the currentSender
                    while (currentRecipientHop.ParentWarehouse.Level < currentSenderHop.ParentWarehouse.Level)
                    {
                        try
                        {
                            currentRecipientHop = _hopRepository.GetHopByCode(recipientWhCode);
                        }
                        catch (DataAccessNotFoundException)
                        {
                            throw new BusinessLogicFutureHopsPrediction($"Warehouse with code {recipientWhCode} does not exist. FutureHops Prediction failed.");
                        }
                        recipientWhCode = AddToListAndFindParent(currentRecipientHop, recipientHopArrivals);
                    }
                }
                else
                {
                    while (currentSenderHop.ParentWarehouse.Level < currentRecipientHop.ParentWarehouse.Level)
                    {
                        try
                        {
                            currentSenderHop = _hopRepository.GetHopByCode(senderWhCode);
                        }
                        catch (DataAccessNotFoundException)
                        {
                            throw new BusinessLogicFutureHopsPrediction($"Warehouse with code {senderWhCode} does not exist. FutureHops Prediction failed.");
                        }
                        senderWhCode = AddToListAndFindParent(currentSenderHop, senderHopArrivals);
                    }
                }
            }

            #endregion
            
            while (senderWhCode != recipientWhCode)
            {
                try
                {
                    currentSenderHop = _hopRepository.GetHopByCode(senderWhCode);
                }
                catch (DataAccessNotFoundException)
                {
                    throw new BusinessLogicFutureHopsPrediction($"Warehouse with code {senderWhCode} does not exist. FutureHops Prediction failed.");
                }
                
                senderWhCode = AddToListAndFindParent(currentSenderHop, senderHopArrivals);

                try
                {
                    currentRecipientHop = _hopRepository.GetHopByCode(recipientWhCode);
                }
                catch (DataAccessNotFoundException)
                {
                    throw new BusinessLogicFutureHopsPrediction($"Warehouse with code {recipientWhCode} does not exist. FutureHops Prediction failed.");
                }

                recipientWhCode = AddToListAndFindParent(currentRecipientHop, recipientHopArrivals);
            }
            
            //add the top Most Wh
            currentSenderHop = _hopRepository.GetHopByCode(senderWhCode);
            senderHopArrivals.Add(new HopArrival()
            {
                Code = currentSenderHop.Code,
                Description = currentSenderHop.Description,
                HopArrivalTime = null
            });
            
            //add the truck/transferWarehouse that delivers the parcel
            var futureHopArrivals = senderHopArrivals.Union(recipientHopArrivals).ToList();
            futureHopArrivals.Add(new HopArrival()
            {
                Code = recipientHop.Code,
                Description = recipientHop.Description,
                HopArrivalTime = null
            });
            
            return futureHopArrivals;
        }

        private string AddToListAndFindParent(DalModels.Hop currentHop, List<HopArrival> currentHopArrivals)
        {
            currentHopArrivals.Add(new HopArrival()
            {
                Code = currentHop.Code,
                Description = currentHop.Description,
                HopArrivalTime = null
            });

            try
            {
                var currentSenderWh = (DalModels.Warehouse) _hopRepository.GetHopByCode(currentHop.ParentWarehouseCode);
                return currentSenderWh.Code;
            }
            catch (DataAccessNotFoundException)
            {
                throw new BusinessLogicFutureHopsPrediction($"Parrenthop with code {currentHop.ParentWarehouseCode} does not exist.");
            }
        }
    }
}