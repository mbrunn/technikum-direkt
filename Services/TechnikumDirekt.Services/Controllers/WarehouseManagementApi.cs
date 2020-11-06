using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
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
    public class WarehouseManagementApiController : ControllerBase
    {
        private IWarehouseLogic _blWarehouseLogic;
        private IMapper _mapper;
        private readonly ILogger _logger;

        public WarehouseManagementApiController(IWarehouseLogic blWarehouseLogic, IMapper automapper,
            ILogger<WarehouseManagementApiController> logger)
        {
            _blWarehouseLogic = blWarehouseLogic;
            _mapper = automapper;
            _logger = logger;
        }

        /// <summary>
        /// Exports the hierarchy of Warehouse and Truck objects. 
        /// </summary>
        /// <response code="200">Successful response</response>
        /// <response code="400">An error occurred loading.</response>
        /// <response code="404">No hierarchy loaded yet.</response>
        [HttpGet]
        [Route("/warehouse")]
        [ValidateModelState]
        [SwaggerOperation("ExportWarehouses")]
        [SwaggerResponse(statusCode: 200, type: typeof(Warehouse), description: "Successful response")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "An error occurred loading.")]
        [SwaggerResponse(statusCode: 404, type: typeof(Error), description: "No hierarchy loaded yet.")]
        public virtual IActionResult ExportWarehouses()
        {
            try
            {
                var exportWarehouse = _blWarehouseLogic.ExportWarehouses();

                if (exportWarehouse == null)
                {
                    _logger.LogWarning("No hierarchy loaded yet.");
                    return NotFound(StatusCode(404, new Error
                    {
                        ErrorMessage = "No hierarchy loaded yet."
                    }));
                }

                _logger.LogInformation("Successfully exported hierarchy.");
                return Ok(_mapper.Map<Warehouse>(exportWarehouse));
            }
            catch (TrackingLogicException e)
            {
                _logger.LogWarning(e.Message);
                return NotFound(StatusCode(404, new Error
                {
                    ErrorMessage = "No hierarchy loaded yet."
                }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(StatusCode(400, 
                    new Error{ ErrorMessage = "An error occured loading."}));
            }
        }

        /// <summary>
        /// Get a certain warehouse or truck by code
        /// </summary>
        /// <param name="code"></param>
        /// <response code="200">Successful response</response>
        /// <response code="400">An error occurred loading.</response>
        /// <response code="404">Warehouse id not found</response>
        [HttpGet]
        [Route("/warehouse/{code}")]
        [ValidateModelState]
        [SwaggerOperation("GetWarehouse")]
        [SwaggerResponse(statusCode: 200, type: typeof(Warehouse), description: "Successful response")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "An error occurred loading.")]
        [SwaggerResponse(statusCode: 404, type: typeof(Error), description: "Warehouse id not found")]
        public virtual IActionResult GetWarehouse([FromRoute][Required][RegularExpression("^[A-Z]{4}\\d{1,4}$")]string code)
        {
            try
            {
                var blWh = _blWarehouseLogic.GetWarehouse(code);
                var svcWarehouse = _mapper.Map<Warehouse>(blWh);
                _logger.LogInformation("Successfully fetched hop with hopcode: "+ code + " - Name: " + svcWarehouse.LocationName);
                return Ok(svcWarehouse);
            }
            catch (FluentValidation.ValidationException e)
            {
                _logger.LogError(e?.Message + " with Value: " + e.Errors.FirstOrDefault()?.AttemptedValue);
                return BadRequest(StatusCode(400, new Error{ ErrorMessage = "An error occured loading."}));
            }
            catch (TrackingLogicException e)
            {
                _logger.LogWarning(e.Message);
                return NotFound(StatusCode(404, new Error
                {
                    ErrorMessage = "Warehouse id not found"
                }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(StatusCode(400, new Error{ ErrorMessage = "An error occured loading."}));
            }
        }
        
        /// <summary>
        /// Imports a hierarchy of Warehouse and Truck objects. 
        /// </summary>
        /// <param name="body"></param>
        /// <response code="200">Successfully loaded.</response>
        /// <response code="400">The operation failed due to an error.</response>
        [HttpPost]
        [Route("/warehouse")]
        [ValidateModelState]
        [SwaggerOperation("ImportWarehouses")]
        [SwaggerResponse(statusCode: 200, type: typeof(Warehouse), description: "Successfully loaded")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult ImportWarehouses([FromBody]Warehouse body)
        {
            try
            {
                var blWh = _mapper.Map<BusinessLogic.Models.Warehouse>(body);
                _blWarehouseLogic.ImportWarehouses(blWh);
                _logger.LogInformation("Successfully imported new warehousestructure");
                return Ok(body);
            }
            catch (FluentValidation.ValidationException e)
            {
                var errorMessage = string.Empty;
                foreach (var error in e.Errors)
                {
                    errorMessage += ("\n" + error?.ErrorMessage + " with Value: " + error?.AttemptedValue);
                }

                _logger.LogError(errorMessage.Trim());
                return BadRequest(StatusCode(400, new Error{ ErrorMessage = "An error occured loading."}));
            }
            catch (TrackingLogicException e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest(StatusCode(400, 
                    new Error{ErrorMessage = "The operation failed due to an error."}));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(StatusCode(400, 
                    new Error{ErrorMessage = "The operation failed due to an error."}));
            }
        }
    }
}