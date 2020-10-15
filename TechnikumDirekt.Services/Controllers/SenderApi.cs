using System;
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
    public class SenderApiController : ControllerBase
    { 
        private ITrackingLogic _trackingLogic;
        private IMapper _mapper;
        public SenderApiController(ITrackingLogic trackingLogic, IMapper mapper)
        {
            _trackingLogic = trackingLogic;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Submit a new parcel to the logistics service. 
        /// </summary>
        /// <param name="body"></param>
        /// <response code="200">Successfully submitted the new parcel</response>
        /// <response code="400">The operation failed due to an error.</response>
        [HttpPost]
        [Route("/parcel")]
        [ValidateModelState]
        [SwaggerOperation("SubmitParcel")]
        [SwaggerResponse(statusCode: 200, type: typeof(NewParcelInfo), description: "Successfully submitted the new parcel")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult SubmitParcel([FromBody]Parcel body)
        {
            try
            {
                if (body != null)
                {
                    var blParcel = _mapper.Map<BusinessLogic.Models.Parcel>(body);
                    _trackingLogic.SubmitParcel(blParcel);
                    return Ok("Successfully submitted the new parcel");
                }
                throw new ArgumentNullException();
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
