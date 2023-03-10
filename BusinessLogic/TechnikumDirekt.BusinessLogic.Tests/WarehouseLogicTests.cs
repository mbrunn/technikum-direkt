using System.Collections.Generic;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.FluentValidation;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.BusinessLogic.Models;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Sql.Exceptions;
using TechnikumDirekt.Services.Mapper;
using DalModels = TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.BusinessLogic.Tests
{
    public class WarehouseLogicTests
    {
        private IMapper _mapper;
        private IWarehouseRepository _warehouseRepository;
        private IWarehouseRepository _emptyWarehouseRepository;
        private IHopRepository _hopRepository;
        private NullLogger<WarehouseLogic> _logger;

        private readonly Warehouse _validWarehouse = new Warehouse
        {
            Code = "WENA04",
            HopType = HopType.Warehouse,
            Description = "Warehouse Level 4 - Wien",
            LocationName = "Wien",
            Level = 4,
            LocationCoordinates = new Point(16.3725042, 48.2083537),
            ProcessingDelayMins = 160
        };

        private readonly Warehouse _invalidWarehouse = new Warehouse
        {
            Code = "WENA04",
            HopType = HopType.Warehouse,
            Description = "Warehouse Level* 4 - Wien",
            LocationName = "Wien",
            Level = 4,
            LocationCoordinates = new Point(16.3725042, 48.2083537),
            ProcessingDelayMins = 160
        };

        private readonly DalModels.Warehouse _validDalWarehouse = new DalModels.Warehouse
        {
            Code = "WENA04",
            HopType = DalModels.HopType.Warehouse,
            Description = "Warehouse Level 4 - Wien",
            LocationName = "Wien",
            Level = 0,
            LocationCoordinates = new Point(16.3725042, 48.2083537),
            ProcessingDelayMins = 160
        };

        private readonly DalModels.Transferwarehouse _validDalTransferWarehouse = new DalModels.Transferwarehouse()
        {
            Code = "TRAN04",
            HopType = DalModels.HopType.Warehouse,
            Description = "Warehouse Level 4 - Wien",
            LocationName = "Wien",
            LocationCoordinates = new Point(16.3725042, 48.2083537),
            ProcessingDelayMins = 160,
            HopArrivals = new List<DalModels.HopArrival>(),
            LogisticsPartner = "Partner 01",
            LogisticsPartnerUrl = "https://www.validurl.at",
            ParentTraveltimeMins = 10
        };

        private readonly List<DalModels.Transferwarehouse>
            _transferwarehouses = new List<DalModels.Transferwarehouse>();
        
        private const string ValidHopCode = "ABCD1234";
        private const string InvalidHopCode = "AbdA2a";

        private IWarehouseLogic _warehouseLogic;
        private IWarehouseLogic _emptyWarehouseLogic;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var mockMapperConfig = new MapperConfiguration(c => c.AddProfile(new DalMapperProfile()));
            _mapper = new Mapper(mockMapperConfig);

            _transferwarehouses.Add(_validDalTransferWarehouse);

            /* ------------- Mock WarehouseRepository Setup ------------- */
            var mockWarehouseRepository = new Mock<IWarehouseRepository>();
            // Setup - GetAll
            mockWarehouseRepository.Setup(m => m.GetAll()).Returns(new List<DalModels.Hop> {_validDalWarehouse});
            // Setup - GetWarehouse
            mockWarehouseRepository.Setup(m => m.GetWarehouseByCode(It.IsAny<string>())).Returns(_validDalWarehouse);
            // Setup - GetTransferWarehouses
            mockWarehouseRepository.Setup(m => m.GetTransferWarehouses()).Returns(_transferwarehouses);

            _warehouseRepository = mockWarehouseRepository.Object;

            /* ------------- Empty Mock WarehouseRepository Setup ------------- */
            var emptyMockWarehouseRepository = new Mock<IWarehouseRepository>();
            // Setup - GetAll
            emptyMockWarehouseRepository.Setup(m => m.GetAll()).Throws(new DataAccessNotFoundException());
            // Setup - GetTransferWarehouses
            emptyMockWarehouseRepository.Setup(m => m.GetTransferWarehouses()).Throws(new DataAccessNotFoundException());
            
            /* --------------- Mock HopRepository Setup --------------- */
            var mockHopRepository = new Mock<IHopRepository>();
            
            mockHopRepository.Setup(m => m.GetHopByCode(It.IsAny<string>())).Returns(_validDalWarehouse);
            
            _emptyWarehouseRepository = emptyMockWarehouseRepository.Object;
            _hopRepository = mockHopRepository.Object;
            
            _logger = NullLogger<WarehouseLogic>.Instance;
        }

        [SetUp]
        public void Setup()
        {
            _warehouseLogic = new WarehouseLogic(new WarehouseValidator(), new HopValidator(),
                _warehouseRepository, _hopRepository, _mapper, _logger);

            _emptyWarehouseLogic = new WarehouseLogic(new WarehouseValidator(), new HopValidator(),
                _emptyWarehouseRepository, _hopRepository, _mapper, _logger);
        }

        #region ExportWarehouses Tests

        [Test]
        public void ExportWarehouses_Throws_WithEmptyWarehouseList()
        {
            Assert.Throws<BusinessLogicNotFoundException>(() => _emptyWarehouseLogic.ExportWarehouses());
        }

        [Test]
        public void ExportWarehouses_DoesNotThrowAndReturnsData_WithNonEmptyWarehouseList()
        {
            _warehouseLogic.ImportWarehouses(_validWarehouse);

            Warehouse warehouses = null;
            Assert.DoesNotThrow(() => warehouses = _warehouseLogic.ExportWarehouses());

            Assert.IsNotNull(warehouses);
        }

        #endregion
        
        #region GetTransferWarehouses Tests

        [Test]
        public void GetTransferWarehouses_Throws_WithEmptyWarehouseList()
        {
            Assert.Throws<BusinessLogicNotFoundException>(() => _emptyWarehouseLogic.GetTransferWarehouses());
        }

        [Test]
        public void GetTransferWarehouses_DoesNotThrowAndReturnsData_WithNonEmptyWarehouseList()
        {
            _warehouseLogic.ImportWarehouses(_validWarehouse);

            IEnumerable<Transferwarehouse> transferwarehouses = new List<Transferwarehouse>();
            Assert.DoesNotThrow(() => transferwarehouses = _warehouseLogic.GetTransferWarehouses());

            Assert.IsNotNull(transferwarehouses);
        }

        #endregion

        #region ImportWarehouses Tests

        [Test]
        public void ImportWarehouses_DoesNotThrow_WithValidWarehouse()
        {
            Assert.DoesNotThrow(() => _warehouseLogic.ImportWarehouses(_validWarehouse));
        }

        [Test]
        public void ImportWarehouses_Throws_WithInvalidWarehouse()
        {
            Assert.Throws<BusinessLogicValidationException>(() => _warehouseLogic.ImportWarehouses(_invalidWarehouse));
        }

        #endregion

        #region GetWarehouse Tests

        [Test]
        public void GetWarehouse_DoesNotThrow_OnValidHopCode()
        {
            Assert.DoesNotThrow(() => _warehouseLogic.GetWarehouse(ValidHopCode));
        }

        [Test]
        public void GetWarehouse_Throws_OnInvalidHopCode()
        {
            Assert.Throws<BusinessLogicValidationException>(() => _warehouseLogic.GetWarehouse(InvalidHopCode));
        }

        #endregion
    }
}