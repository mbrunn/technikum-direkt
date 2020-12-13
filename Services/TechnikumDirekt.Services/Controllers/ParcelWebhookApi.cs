using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.Services.Attributes;
using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Services.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class ParcelWebhookApiController : ControllerBase
    {
        private ITrackingLogic _trackingLogic;
        private IMapper _mapper;
        private ILogger _logger;
        public ParcelWebhookApiController(IMapper mapper,
            ILogger<ParcelWebhookApiController> logger, ITrackingLogic trackingLogic)
        {
            _trackingLogic = trackingLogic;
            _mapper = mapper;
            _logger = logger;
        }
        /// <summary>
        /// Get all registered subscriptions for the parcel trackingId.
        /// </summary>
        /// <param name="trackingId"></param>
        /// <response code="200">List of webooks for the &#x60;trackingId&#x60;</response>
        /// <response code="404">No parcel found with that tracking ID.</response>
        [HttpGet]
        [Route("/parcel/{trackingId}/webhooks")]
        [ValidateModelState]
        [Produces("application/json")]
        [SwaggerOperation("ListParcelWebhooks")]
        [SwaggerResponse(statusCode: 200, type: typeof(WebhookResponses),
            description: "List of webhooks for the &#x60;trackingId&#x60;")]
        public virtual IActionResult ListParcelWebhooks([FromRoute] [Required] [RegularExpression("^[A-Z0-9]{9}$")]
            string trackingId)
        {
            try
            {
                var blWebhooks = _trackingLogic.GetAllSubscribersByTrackingId(trackingId);
                var webhooks = _mapper.Map<List<WebhookResponse>>(blWebhooks);
                return Ok( webhooks);
            }
            catch (BusinessLogicNotFoundException)
            {
                _logger.LogDebug($"No parcel found with trackingID: {trackingId}");
                return NotFound(new Error
                {
                    ErrorMessage = $"No parcel found with that tracking ID."
                });
            }
            catch (BusinessLogicValidationException)
            {
                _logger.LogWarning($"trackingId: {trackingId} is invalid.");
                return BadRequest(new Error
                {
                    ErrorMessage = $"trackingId: {trackingId} is invalid."
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(new Error
                {
                    ErrorMessage = "The operation failed due to an error."
                });
            } 
        }
        
        /// <summary>
        /// Subscribe to a webhook notification for the specific parcel.
        /// </summary>
        /// <param name="trackingId"></param>
        /// <param name="url"></param>
        /// <response code="200">Successful response</response>
        /// <response code="404">No parcel found with that tracking ID.</response>
        [HttpPost]
        [Route("/parcel/{trackingId}/webhooks")]
        [ValidateModelState]
        [SwaggerOperation("SubscribeParcelWebhook")]
        [SwaggerResponse(statusCode: 200, type: typeof(WebhookResponse), description: "Successful response")]
        public virtual IActionResult SubscribeParcelWebhook([FromRoute] [Required] [RegularExpression("^[A-Z0-9]{9}$")]
            string trackingId, [FromQuery] [Required()] string url)
        {
            try
            {
                var blWebhook = _trackingLogic.SubscribeParcelWebhook(trackingId, url);
                var webhookResponse = _mapper.Map<WebhookResponse>(blWebhook);
                return Ok( webhookResponse);
            }
            catch (BusinessLogicNotFoundException)
            {
                _logger.LogDebug($"Parcel with trackingId: {trackingId} does not exist. WebhookSubscription failed.");
                return NotFound(new Error
                {
                    ErrorMessage = $"No parcel found with that tracking ID."
                });
            }
            catch (BusinessLogicValidationException)
            {
                _logger.LogWarning($"trackingId: {trackingId} is invalid.");
                return BadRequest(new Error
                {
                    ErrorMessage = $"trackingId: {trackingId} is invalid."
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(new Error
                {
                    ErrorMessage = "The operation failed due to an error."
                });
            }
        }

        /// <summary>
        /// Remove an existing webhook subscription.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Success</response>
        /// <response code="404">Subscription does not exist.</response>
        [HttpDelete]
        [Route("/parcel/webhooks/{id}")]
        [ValidateModelState]
        [SwaggerOperation("UnsubscribeParcelWebhook")]
        public virtual IActionResult UnsubscribeParcelWebhook([FromRoute] [Required] long id)
        {
            try
            {
                _trackingLogic.RemoveParcelWebhook(id);
                return Ok();
            }
            catch (BusinessLogicNotFoundException)
            {
                _logger.LogDebug($"webhook with Id {id} does not exist.  WebhookRemoval failed.");
                return NotFound(new Error
                {
                    ErrorMessage = $"No webhook found with that ID."
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(new Error
                {
                    ErrorMessage = "The operation failed due to an error."
                });
            }
        }
    }
}