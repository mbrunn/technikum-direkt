using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.Services.Attributes;
using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Services.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class LogisticsPartnerApiController : ControllerBase
    { 
        private ITrackingLogic _trackingLogic;
        private IMapper _mapper;
        public LogisticsPartnerApiController(ITrackingLogic trackingLogic, IMapper mapper)
        {
            _trackingLogic = trackingLogic;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Transfer an existing parcel into the system from the service of a logistics partner. 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="trackingId">The tracking ID of the parcel. E.g. PYJRB4HZ6 </param>
        /// <response code="200">Successfully transitioned the parcel</response>
        /// <response code="400">The operation failed due to an error.</response>
        [HttpPost]
        [Route("/parcel/{trackingId}")]
        [ValidateModelState]
        [SwaggerOperation("TransitionParcel")]
        [SwaggerResponse(statusCode: 200, type: typeof(NewParcelInfo), description: "Successfully transitioned the parcel")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public IActionResult TransitionParcel([FromBody]Parcel body, [FromRoute][Required][RegularExpression("^[A-Z0-9]{9}$")]string trackingId)
        { 
            try
            {
                var blParcel = _mapper.Map<BusinessLogic.Models.Parcel>(body);
                _trackingLogic.TransitionParcelFromPartner(blParcel, trackingId);
                return Ok("Successfully transitioned the parcel");
            }
            
            catch
            {
                return BadRequest(StatusCode(400, new Error
                {
                    ErrorMessage = "The operation failed due to an error."
                }));  
            }
        }
    }
}