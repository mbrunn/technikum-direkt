using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using AutoMapper;
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
        public WarehouseManagementApiController(IWarehouseLogic blWarehouseLogic, IMapper automapper)
        {
            _blWarehouseLogic = blWarehouseLogic;
            _mapper = automapper;
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
           //Exceptionhandling for statusCode 400.
           try
           {
               var exportWarehouses = _blWarehouseLogic.ExportWarehouses().ToList();

               if (!exportWarehouses.Any())
               {
                   return NotFound(StatusCode(404, new Error
                   {
                       ErrorMessage = "No hierarchy loaded yet."
                   }));
               }

               var whResponseList = new List<Warehouse>();
               Warehouse svcWarehouses;

               foreach (var wh in exportWarehouses)
               {
                   svcWarehouses = _mapper.Map<Warehouse>(wh);
                   if (svcWarehouses != null)
                   {
                       whResponseList.Add(svcWarehouses);
                   }
               }
               return Ok(whResponseList);
           }
           
           catch (Exception)
           {
               return BadRequest(StatusCode(400, new Error{ ErrorMessage = "An error occured loading."}));
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
                    
                if (blWh != null)
                {
                    var svcWarehouse = _mapper.Map<Warehouse>(blWh);
                    if (svcWarehouse != null)
                    {
                        return Ok(svcWarehouse);
                    }
                }
                
                return NotFound(StatusCode(404, new Error
                {
                    ErrorMessage = "Warehouse id not found!"
                }));
            }
            
            catch
            {
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
                return Ok(body);
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
