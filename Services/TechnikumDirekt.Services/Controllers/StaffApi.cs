using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.Services.Attributes;
using TechnikumDirekt.Services.Models;
using ValidationException = FluentValidation.ValidationException;

namespace TechnikumDirekt.Services.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class StaffApiController : ControllerBase
    {
        private readonly ITrackingLogic _trackingLogic;
        private readonly ILogger _logger;

        public StaffApiController(ITrackingLogic trackingLogic, ILogger<StaffApiController> logger)
        {
            _trackingLogic = trackingLogic;
            _logger = logger;
        }

        /// <summary>
        /// Report that a Parcel has been delivered at it&#x27;s final destination address. 
        /// </summary>
        /// <param name="trackingId">The tracking ID of the parcel. E.g. PYJRB4HZ6 </param>
        /// <response code="200">Successfully reported hop.</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="404">Parcel does not exist with this tracking ID. </response>
        [HttpPost]
        [Route("/parcel/{trackingId}/reportDelivery/")]
        [ValidateModelState]
        [SwaggerOperation("ReportParcelDelivery")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult ReportParcelDelivery([FromRoute] [Required] [RegularExpression("^[A-Z0-9]{9}$")]
            string trackingId)
        {
            try
            {
                _trackingLogic.ReportParcelDelivery(trackingId);
                _logger.LogInformation($"Successfully reported hop with TrackingId: " + trackingId);
                return Ok("Successfully reported hop.");
            }
            catch (BusinessLogicNotFoundException e)
            {
                _logger.LogWarning(e.Message);
                return NotFound(StatusCode(404, new Error
                {
                    ErrorMessage = e.Message
                }));
            }
            catch (BusinessLogicValidationException e)
            {
                _logger.LogWarning(e?.Message);
                return BadRequest(StatusCode(400, new Error
                {
                    ErrorMessage = "The operation failed due to an error."
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

        /// <summary>
        /// Report that a Parcel has arrived at a certain hop either Warehouse or Truck. 
        /// </summary>
        /// <param name="trackingId">The tracking ID of the parcel. E.g. PYJRB4HZ6 </param>
        /// <param name="code">The Code of the hop (Warehouse or Truck).</param>
        /// <response code="200">Successfully reported hop.</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="404">Parcel does not exist with this tracking ID or hop with code not found.</response>
        [HttpPost]
        [Route("/parcel/{trackingId}/reportHop/{code}")]
        [ValidateModelState]
        [SwaggerOperation("ReportParcelHop")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult ReportParcelHop([FromRoute] [Required] [RegularExpression("^[A-Z0-9]{9}$")]
            string trackingId, [FromRoute] [Required] [RegularExpression("^[A-Z]{4}\\d{1,4}$")]
            string code)
        {
            try
            {
                _trackingLogic.ReportParcelHop(trackingId, code);
                _logger.LogInformation("Successfully reported hop with hopcode: " + code + " and trackingId: " +
                                       trackingId);
                return Ok("Successfully reported hop.");
            }
            catch (BusinessLogicNotFoundException e)
            {
                _logger.LogWarning(e.Message);
                return NotFound(StatusCode(404, new Error
                {
                    ErrorMessage = e.Message
                }));
            }
            catch (BusinessLogicValidationException e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest(StatusCode(400, new Error
                {
                    ErrorMessage = "The operation failed due to an error."
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