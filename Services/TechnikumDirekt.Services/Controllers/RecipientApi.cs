/*
 * Parcel Logistics Service
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: 1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
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
    public class RecipientApiController : ControllerBase
    {
        private ITrackingLogic _trackingLogic;
        private IMapper _mapper;
        public RecipientApiController(ITrackingLogic trackingLogic, IMapper mapper)
        {
            _trackingLogic = trackingLogic;
            _mapper = mapper;
        }

        /// <summary>
        /// Find the latest state of a parcel by its tracking ID. 
        /// </summary>
        /// <param name="trackingId">The tracking ID of the parcel. E.g. PYJRB4HZ6 </param>
        /// <response code="200">Parcel exists, here&#x27;s the tracking information.</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="404">Parcel does not exist with this tracking ID.</response>
        [HttpGet]
        [Route("/parcel/{trackingId}")]
        [ValidateModelState]
        [SwaggerOperation("TrackParcel")]
        [SwaggerResponse(statusCode: 200, type: typeof(TrackingInformation), description: "Parcel exists, here&#x27;s the tracking information.")]
        
        public virtual IActionResult TrackParcel([FromRoute][Required][RegularExpression("^[A-Z0-9]{9}$")]string trackingId)
        {
            try
            {
                var tlParcel = _trackingLogic.TrackParcel(trackingId);
                
                var svcTrackingInformation = _mapper.Map<TrackingInformation>(tlParcel);

                return Ok(svcTrackingInformation); //TODO add Msg to response
            }
            catch (TrackingLogicException)
            {
                return NotFound(StatusCode(404, new Error
                {
                    ErrorMessage = "No hierarchy loaded yet."
                }));
            }
            catch
            {
                return BadRequest(StatusCode(400, new Error {ErrorMessage = "The operation failed due to an error."}));
            }
        }
    }
}
