using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TechnikumDirekt.Services.Controllers;
using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Tests
{
    [TestFixture]
    public class WarehouseManagementApiTests
    {
        [Test]
        public void ExportWarehouses_Valid_Ok()
        {
            var controller = new WarehouseManagementApiController();

            var response = controller.ExportWarehouses();

            Assert.IsInstanceOf<OkObjectResult>(response);

            object body = ((OkObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 200);
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [Test]
        public void GetWarehouse_ValidHopCode_Ok()
        {
            var controller = new WarehouseManagementApiController();

            var response = controller.GetWarehouse("Benjis Warehouse");

            Assert.IsInstanceOf<OkObjectResult>(response);

            object body = ((OkObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 200);
        }
        
        [Test]
        public void GetWarehouse_InvalidHopCode_NotFound()
        {
            var controller = new WarehouseManagementApiController();

            var response = controller.GetWarehouse("NonExistingHopCode");

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            object body = ((NotFoundObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 404);
        }
        
        [Test]
        public void GetWarehouse_NullHopCode_BadRequest()
        {
            var controller = new WarehouseManagementApiController();

            var response = controller.GetWarehouse(null);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            object body = ((BadRequestObjectResult) response).Value;

            Assert.IsNotNull(body);
            
            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 400);
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [Test]
        public void ImportWarehouses_ValidBody_Ok()
        {
            var controller = new WarehouseManagementApiController();
            var warehouse = new Warehouse
            {
                HopType = "Warehouse",
                Code = "12345",
                Description = "Descriptive description",
                ProcessingDelayMins = 100,
                LocationName = "Benjis Hut",
                LocationCoordinates = new GeoCoordinate
                {
                    Lat = 43.645074,
                    Lon = -115.993081
                },
                Level = 5,
                NextHops = new List<WarehouseNextHops>()
            };
            
            var response = controller.ImportWarehouses(warehouse);

            Assert.IsInstanceOf<OkObjectResult>(response);

            object body = ((OkObjectResult) response).Value;

            int? statusCode = ((StatusCodeResult) body).StatusCode;

            Assert.IsTrue(statusCode == 200);
        }
        
        [Test]
        public void ImportWarehouses_InValidBody_BadRequest()
        {
            var controller = new WarehouseManagementApiController();
            Warehouse warehouse = null;
            
            var response = controller.ImportWarehouses(warehouse);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            object body = ((BadRequestObjectResult) response).Value;

            int? statusCode = ((ObjectResult) body).StatusCode;

            Assert.IsTrue(statusCode == 400);
        }
    }
}