using System;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.ServiceAgents.Interfaces;
using TechnikumDirekt.ServiceAgents.Models;
using TechnikumDirekt.Services.Attributes;
using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Services.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class SenderApiController : ControllerBase
    {
        private ITrackingLogic _trackingLogic;
        private IMapper _mapper;
        private ILogger _logger;
        public SenderApiController(ITrackingLogic trackingLogic, IMapper mapper, ILogger<SenderApiController> logger)
        {
            _trackingLogic = trackingLogic;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Submit a new parcel to the logistics service. 
        /// </summary>
        /// <param name="body"></param>
        /// <response code="200">Successfully submitted the new parcel,</response>
        /// <response code="400">The operation failed due to an error.</response>
        [HttpPost]
        [Route("/parcel")]
        [ValidateModelState]
        [SwaggerOperation("SubmitParcel")]
        [SwaggerResponse(statusCode: 200, type: typeof(NewParcelInfo),
            description: "Successfully submitted the new parcel")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult SubmitParcel([FromBody] Parcel body)
        {
            try
            {
                var blParcel = _mapper.Map<BusinessLogic.Models.Parcel>(body);
                var trackingId = _trackingLogic.SubmitParcel(blParcel);
                var newParcelInfo = new NewParcelInfo() {TrackingId = trackingId};
                _logger.LogInformation("Successfully submitted a new parcel with trackingId: " + trackingId);
                return Ok(newParcelInfo);
            }
            catch (BusinessLogicValidationException)
            {
                var errorMessage = string.Empty;
                /*foreach (var error in e.Errors)
                {
                    errorMessage += ("\n" + error?.ErrorMessage + " with Value: " + error?.AttemptedValue);
                }*/

                _logger.LogWarning(errorMessage.Trim());
                return BadRequest(StatusCode(400, new Error
                {
                    ErrorMessage = "The parcel has invalid data."
                }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(StatusCode(400, new Error
                {
                    ErrorMessage = "The operation failed due to an error."
                }));
            }
        }
    }
}