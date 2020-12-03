using System;
using System.Collections.Generic;
using System.Linq;
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
using TechnikumDirekt.Services.Models;
using DalModels = TechnikumDirekt.DataAccess.Models;
using Hop = TechnikumDirekt.BusinessLogic.Models.Hop;
using HopArrival = TechnikumDirekt.BusinessLogic.Models.HopArrival;
using Parcel = TechnikumDirekt.BusinessLogic.Models.Parcel;
using Recipient = TechnikumDirekt.BusinessLogic.Models.Recipient;
using Transferwarehouse = TechnikumDirekt.BusinessLogic.Models.Transferwarehouse;

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
        private readonly IWebhookRepository _webhookRepository;

        private readonly IGeoEncodingAgent _geoEncodingAgent;
        private readonly ILogisticsPartnerAgent _logisticsPartnerAgent;
        private readonly IWebhookServiceAgent _webhookServiceAgent;
        
        private readonly IMapper _mapper;

        private readonly ILogger _logger;

        public TrackingLogic(IValidator<Parcel> parcelValidator, IValidator<Recipient> recipientValidator,
            IValidator<HopArrival> hopArrivalValidator,
            IValidator<Hop> hopCodeValidator, IHopRepository hopRepository, IParcelRepository parcelRepository, IWebhookRepository webhookRepository,
            IGeoEncodingAgent geoEncodingAgent, ILogisticsPartnerAgent logisticsPartnerAgent, IWebhookServiceAgent webhookServiceAgent, IWarehouseLogic warehouseLogic, IMapper mapper, 
            ILogger<TrackingLogic> logger)
        {
            _parcelValidator = parcelValidator;
            _recipientValidator = recipientValidator;
            _hopArrivalValidator = hopArrivalValidator;
            _hopCodeValidator = hopCodeValidator;
            _hopRepository = hopRepository;
            _parcelRepository = parcelRepository;
            _webhookRepository = webhookRepository;
            _geoEncodingAgent = geoEncodingAgent;
            _logisticsPartnerAgent = logisticsPartnerAgent;
            _webhookServiceAgent = webhookServiceAgent;
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
                _logger.LogDebug($"Parcel has invalid trackingId: {trackingId}");
                throw new BusinessLogicValidationException("Parcel validation failed.", e);
            }

            try
            {
                var parcel = _parcelRepository.GetByTrackingId(trackingId);
                if (parcel.State == DalModels.Parcel.StateEnum.DeliveredEnum)
                {
                    _logger.LogDebug($"Parcel with TrackingId {trackingId} has already been delivered.");
                }
                else
                {
                    parcel.State = DalModels.Parcel.StateEnum.DeliveredEnum;
                    parcel.HopArrivals.OrderBy(ha => ha.Order).Last().HopArrivalTime = DateTime.Now;
                    
                    //call all webhookSubscribers
                    NotifyWebhookSubscribers(parcel.TrackingId);
                    
                    _parcelRepository.Update(parcel);
                    _logger.LogDebug($"Parcel with TrackingId {trackingId} has been set to delivered.");
                }
            }
            catch (DataAccessNotFoundException e)
            {
                _logger.LogDebug($"Parcel for TrackingId {trackingId} not found");
                throw new BusinessLogicNotFoundException($"Parcel for TrackingId {trackingId} not found", e);
            }
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
                _logger.LogDebug($"Hop has invalid hopCode: {code}");
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
                _logger.LogDebug($"Parcel has invalid trackingId: {trackingId}");
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
            
            //call all webhookSubscribers
            NotifyWebhookSubscribers(parcel.TrackingId);

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
                _logger.LogDebug($"Parcel has invalid data.");
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
                _logger.LogDebug($"Parcel has invalid trackingId: {trackingId}");
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
                _logger.LogDebug($"Parcel has invalid data.");
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
            //1. get Coordinates of Sender and Recipient:
            if (parcel.Sender == null)
            {
                _logger.LogTrace($"Sender cant be null.");
                throw new BusinessLogicBadArgumentException($"Sender can not be null.");
            }
            
            if (parcel.Recipient == null)
            {
                _logger.LogTrace($"Recipient cant be null.");
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
                _logger.LogTrace($"Address of Sender {parcel.Sender.Name} could not be decoded into GPS Coordinates.");
                throw new BusinessLogicFutureHopsPrediction($"Address of Sender {parcel.Sender.Name} could not be decoded into GPS Coordinates.");
            }
            
            Point recipientPoint;

            try
            {
                recipientPoint = _geoEncodingAgent.EncodeAddress(recipientAddress);
            }
            catch (ServiceAgentsNotFoundException e)
            {
                _logger.LogTrace($"Address of Sender {parcel.Recipient.Name} could not be decoded into GPS Coordinates.");
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
                _logger.LogTrace($"No Hop contains the given Point {senderPoint.AsText()} could not be decoded into GPS Coordinates.");
                throw new BusinessLogicFutureHopsPrediction($"No Hop contains the given Point {senderPoint.AsText()} could not be decoded into GPS Coordinates.");
            }

            Hop nearestHoptoReceipient;
            
            try
            { 
                nearestHoptoReceipient = _warehouseLogic.GetHopContainingPoint(recipientPoint);
            }
            catch (DataAccessArgumentNullException e)
            {
                _logger.LogTrace($"No Hop contains the given Point {recipientPoint.AsText()} could not be decoded into GPS Coordinates.");
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
                _logger.LogTrace($"Nearest Hop to Sender with code {senderHop.Code} does not exist. FutureHops Prediction failed.");
                throw new BusinessLogicFutureHopsPrediction($"Nearest Hop to Sender with code {senderHop.Code} does not exist. FutureHops Prediction failed.");
            }
            
            try
            {
                currentRecipientHop = _hopRepository.GetHopByCode(recipientHop.Code);
            }
            catch (DataAccessNotFoundException)
            {
                _logger.LogTrace($"Nearest Hop to Recipient with code {recipientHop.Code} does not exist. FutureHops Prediction failed.");
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
                _logger.LogTrace($"Warehouse with code {senderWhCode} does not exist. FutureHops Prediction failed.");
                throw new BusinessLogicFutureHopsPrediction($"Warehouse with code {senderWhCode} does not exist. FutureHops Prediction failed.");
            }
            
            try
            {
                recipientWh = (DalModels.Warehouse) _hopRepository.GetHopByCode(recipientWhCode);
            }
            catch (DataAccessNotFoundException)
            {
                _logger.LogTrace($"Warehouse with code {recipientWhCode} does not exist. FutureHops Prediction failed.");
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
                            _logger.LogTrace($"Warehouse with code {recipientWhCode} does not exist. FutureHops Prediction failed.");
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
                            _logger.LogTrace($"Warehouse with code {senderWhCode} does not exist. FutureHops Prediction failed.");
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
                    _logger.LogTrace($"Warehouse with code {senderWhCode} does not exist. FutureHops Prediction failed.");
                    throw new BusinessLogicFutureHopsPrediction($"Warehouse with code {senderWhCode} does not exist. FutureHops Prediction failed.");
                }
                
                senderWhCode = AddToListAndFindParent(currentSenderHop, senderHopArrivals);

                try
                {
                    currentRecipientHop = _hopRepository.GetHopByCode(recipientWhCode);
                }
                catch (DataAccessNotFoundException)
                {
                    _logger.LogTrace($"Warehouse with code {recipientWhCode} does not exist. FutureHops Prediction failed.");
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

            //turn around recipientHopArrivals
            recipientHopArrivals.Reverse();
            
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
                _logger.LogTrace($"Parrenthop with code {currentHop.ParentWarehouseCode} does not exist.");
                throw new BusinessLogicFutureHopsPrediction($"Parrenthop with code {currentHop.ParentWarehouseCode} does not exist.");
            }
        }

        /// <summary>
        /// Calls all webhookSubscribers that are subscribed to the given trackingId
        /// </summary>
        /// <param name="trackingId">trackingId of tracked parcel</param>
        private void NotifyWebhookSubscribers(string trackingId)
        {
            var webhooks =
                _mapper.Map<List<Webhook>>(_webhookRepository.GetAllSubscribersByTrackingId(trackingId));
            
            foreach (var webhook in webhooks)
            {
                try
                {
                    _webhookServiceAgent.NotifySubscriber(webhook);
                }
                catch (ServiceAgentsBadResponseException e)
                {
                    //TODO: What shall we do with the broken URLs ?
                    _logger.LogDebug($"Webhook with URL: {webhook.Url} didn't return a valid response.");
                }
            }
        }

        /// <summary>
        /// retrieves all webhookSubscriptions from the database
        /// </summary>
        /// <param name="trackingId"></param>
        /// <returns></returns>
        /// <exception cref="BusinessLogicValidationException">Is thrown if trackingId is in an invalid format.</exception>
        /// <exception cref="BusinessLogicBadArgumentException">Is thrown if no webhooks for given trackingId exist.</exception>
        public IEnumerable<Webhook> GetAllSubscribersByTrackingId(string trackingId)
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
                _logger.LogDebug($"Parcel has invalid trackingId: {trackingId}");
                throw new BusinessLogicValidationException("Parcel validation failed.", e);
            }
            
            try
            {
                _parcelRepository.GetByTrackingId(trackingId);
                var webhooks = _mapper.Map<List<Webhook>>(_webhookRepository.GetAllSubscribersByTrackingId(trackingId));
                _logger.LogDebug($"Found {webhooks.Count()} webhookSubscriptions for parcel with trackingId: {trackingId}");
                
                return webhooks;
            }
            catch (DataAccessNotFoundException e)
            {
                _logger.LogTrace($"Webhooks for trackingId {trackingId} do not exist.");
                throw new BusinessLogicNotFoundException($"Webhooks for trackingId {trackingId} do not exist.");
            }
        }

        public Webhook SubscribeParcelWebhook(string trackingId, string url)
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
                _logger.LogDebug($"Invalid trackingId: {trackingId}");
                throw new BusinessLogicValidationException("TrackingId validation failed.", e);
            }
            
            var parcel = TrackParcel(trackingId);
            
            try
            {
                var dalWebhook = new DalModels.Webhook()
                {
                    CreationDate = DateTime.Now,
                    Parcel = _parcelRepository.GetByTrackingId(trackingId),
                    Url = url
                };
                
                _webhookRepository.AddSubscription(dalWebhook);
                
                _logger.LogDebug($"Added subscription for parcel with trackingId: {trackingId} and URL: {dalWebhook.Url}");

                return _mapper.Map<Webhook>(dalWebhook);
            }
            catch (DataAccessNotFoundException e)
            {
                _logger.LogTrace($"Parcel with trackingId {trackingId} does not exist.  WebhookSubscription failed.");
                throw new BusinessLogicBadArgumentException($"Parcel with trackingId {trackingId} does not exist. WebhookSubscription failed.");
            }
        }

        public void RemoveParcelWebhook(long id)
        {
            try
            {
                _webhookRepository.RemoveSubscription(id);
                
                _logger.LogDebug($"Removed webhook with Id: {id}");
            }
            catch (DataAccessNotFoundException e)
            {
                _logger.LogTrace($"Webhook with Id {id} does not exist.  WebhookRemoval failed.");
                throw new BusinessLogicNotFoundException($"Webhook with Id {id} does not exist.  WebhookRemoval failed.");
            }
        }
    }
}