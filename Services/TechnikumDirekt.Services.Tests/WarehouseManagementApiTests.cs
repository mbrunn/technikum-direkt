using System.Collections.Generic;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.BusinessLogic.Models;
using TechnikumDirekt.Services.Controllers;
using TechnikumDirekt.Services.Mapper;
using TechnikumDirekt.Services.Models;
using BlWarehouse = TechnikumDirekt.BusinessLogic.Models.Warehouse;
using BlTransferWarehouse = TechnikumDirekt.BusinessLogic.Models.Transferwarehouse;
using Warehouse = TechnikumDirekt.Services.Models.Warehouse;
using WarehouseNextHops = TechnikumDirekt.Services.Models.WarehouseNextHops;

namespace TechnikumDirekt.Services.Tests
{
    [TestFixture]
    public class WarehouseManagementApiTests
    {
        private IWarehouseLogic _warehouseLogic;
        private IWarehouseLogic _emptyWarehouseLogic;
        private IMapper _mapper;
        private NullLogger<WarehouseManagementApiController> _logger;

        private readonly BlWarehouse _validWarehouse = new BlWarehouse
        {
            Code = "WENA04",
            HopType = HopType.Warehouse,
            Description = "Warehouse Level 4 - Wien",
            LocationName = "Wien",
            Level = 4,
            LocationCoordinates = new Point(16.3725042, 48.2083537),
            ProcessingDelayMins = 160,
            NextHops = new List<BusinessLogic.Models.WarehouseNextHops>()
        };
        
        private readonly BlTransferWarehouse _validTransferWarehouse = new BlTransferWarehouse
        {
            Code = "WENA04",
            HopType = HopType.TransferWarehouse,
            Description = "Transferwarehouse Lanzendorf",
            LocationName = "Lanzendorf",
            LocationCoordinates = new Point(16.3725042, 48.2083537),
            ProcessingDelayMins = 160,
            LogisticsPartner = "Lanzendorf TransferWarehosue",
            LogisticsPartnerUrl = "http://www.validurl.at",
            RegionGeometry = null
        };
        
        private readonly List<BlWarehouse> _warehouses = new List<BlWarehouse>();
        private readonly List<BlTransferWarehouse> _transferWarehouses = new List<BlTransferWarehouse>();

        private const string ValidHopCode = "ABCD1234";
        private const string InvalidHopCode = "AbdA2a";
        private const string NotfoundHopCode = "ABCD0000";

        [OneTimeSetUp]
        public void Setup()
        {
            var mockMapperConfig = new MapperConfiguration(c => c.AddProfile(new BlMapperProfile()));
            _mapper = new AutoMapper.Mapper(mockMapperConfig);

            _warehouses.Add(_validWarehouse);
            _transferWarehouses.Add(_validTransferWarehouse);
            
            var mockWarehouseLogic = new Mock<IWarehouseLogic>();
            // Setup - ExportWarehouses
            mockWarehouseLogic.Setup(m => m.ExportWarehouses()).Returns(_validWarehouse);
            mockWarehouseLogic.Setup(m => m.GetTransferWarehouses()).Returns(_transferWarehouses);
            mockWarehouseLogic.Setup(m => m.GetWarehouse(ValidHopCode)).Returns(_validWarehouse);
            mockWarehouseLogic.Setup(m => m.GetWarehouse(InvalidHopCode)).Throws<BusinessLogicValidationException>();
            mockWarehouseLogic.Setup(m => m.GetWarehouse(NotfoundHopCode)).Throws<BusinessLogicNotFoundException>();
            mockWarehouseLogic.Setup(m => m.ImportWarehouses(It.IsAny<BlWarehouse>()));
            mockWarehouseLogic.Setup(m => m.ImportWarehouses(null)).Throws<BusinessLogicValidationException>();

            var emptyMockWarehouseLogic = new Mock<IWarehouseLogic>();
            // Setup - ExportWarehouses
            emptyMockWarehouseLogic.Setup(m => m.ExportWarehouses()).Throws<BusinessLogicNotFoundException>();
            emptyMockWarehouseLogic.Setup(m => m.GetTransferWarehouses()).Throws<BusinessLogicNotFoundException>();

            _warehouseLogic = mockWarehouseLogic.Object;
            _emptyWarehouseLogic = emptyMockWarehouseLogic.Object;
            _logger = NullLogger<WarehouseManagementApiController>.Instance;
        }

        #region ExportWarehouses Tests

        [Test]
        public void ExportWarehouses_Valid_Ok()
        {
            var controller = new WarehouseManagementApiController(_warehouseLogic, _mapper, _logger);

            var response = controller.ExportWarehouses();

            Assert.IsInstanceOf<OkObjectResult>(response);

            var typedResponse = (OkObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(200, statusCode);
        }

        [Test]
        public void ExportWarehouses_Empty_Notfound()
        {
            var controller = new WarehouseManagementApiController(_emptyWarehouseLogic, _mapper, _logger);

            var response = controller.ExportWarehouses();

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            var typedResponse = (NotFoundObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(404, statusCode);
        }

        [Test]
        public void ExportWarehouse_BlReturnsNull_NotFound()
        {
            var controller = new WarehouseManagementApiController(_emptyWarehouseLogic, _mapper, _logger);

            var response = controller.ExportWarehouses();

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            var typedResponse = (NotFoundObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(404, statusCode);
        }

        #endregion
        
        #region GetTransferWarehouses Tests

        [Test]
        public void GetTransferWarehouses_Valid_Ok()
        {
            var controller = new WarehouseManagementApiController(_warehouseLogic, _mapper, _logger);

            var response = controller.GetTransferWarehouses();

            Assert.IsInstanceOf<OkObjectResult>(response);

            var typedResponse = (OkObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(200, statusCode);
        }

        [Test]
        public void GetTransferWarehouses_Empty_Notfound()
        {
            var controller = new WarehouseManagementApiController(_emptyWarehouseLogic, _mapper, _logger);

            var response = controller.GetTransferWarehouses();

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            var typedResponse = (NotFoundObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(404, statusCode);
        }

        [Test]
        public void GetTransferWarehouses_BlReturnsNull_NotFound()
        {
            var controller = new WarehouseManagementApiController(_emptyWarehouseLogic, _mapper, _logger);

            var response = controller.GetTransferWarehouses();

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            var typedResponse = (NotFoundObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(404, statusCode);
        }

        #endregion

        #region GetWarehouse Tests

        [Test]
        public void GetWarehouse_ValidHopCode_Ok()
        {
            var controller = new WarehouseManagementApiController(_warehouseLogic, _mapper, _logger);

            var response = controller.GetWarehouse(ValidHopCode);

            Assert.IsInstanceOf<OkObjectResult>(response);

            var typedResponse = (OkObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(200, statusCode);
        }

        [Test]
        public void GetWarehouse_NonexistentHopCode_NotFound()
        {
            var controller = new WarehouseManagementApiController(_warehouseLogic, _mapper, _logger);

            var response = controller.GetWarehouse(NotfoundHopCode);

            Assert.IsInstanceOf<NotFoundObjectResult>(response);

            var typedResponse = (NotFoundObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(404, statusCode);
        }

        [Test]
        public void GetWarehouse_InvalidHopCode_BadRequest()
        {
            var controller = new WarehouseManagementApiController(_warehouseLogic, _mapper, _logger);

            var response = controller.GetWarehouse(InvalidHopCode);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            var typedResponse = (BadRequestObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(400, statusCode);
        }

        #endregion

        #region ImportWarehouses Tests

        [Test]
        public void ImportWarehouses_ValidBody_Ok()
        {
            var controller = new WarehouseManagementApiController(_warehouseLogic, _mapper, _logger);
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

            var typedResponse = (OkObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(200, statusCode);
        }

        [Test]
        public void ImportWarehouses_InValidBody_BadRequest()
        {
            var controller = new WarehouseManagementApiController(_warehouseLogic, _mapper, _logger);

            var response = controller.ImportWarehouses(null);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            var typedResponse = (BadRequestObjectResult) response;
            var statusCode = typedResponse.StatusCode;

            Assert.AreEqual(400, statusCode);
        }

        #endregion
    }
}