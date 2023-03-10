using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;
using TechnikumDirekt.DataAccess.Sql;
using TechnikumDirekt.DataAccess.Sql.Exceptions;

namespace TechnikumDirekt.DataAccess.Tests
{
    public class WarehouseRepositoryTests
    {
        private ITechnikumDirektContext _technikumDirektContext;
        private IWarehouseRepository _warehouseRepository;
        private List<Hop> _entities;
        private NullLogger<WarehouseRepository> _logger;

        private const string ValidHopCode = "AUTA05";
        private const string InvalidHopCode = "AUTA99";

        private static readonly Transferwarehouse ValidTransferWarehouse01 = new Transferwarehouse
        {
            Code = "TRAN01",
            HopType = HopType.TransferWarehouse,
            Description = "TransferWarehouse - Location01",
            LocationName = "Location01",
            LocationCoordinates = new Point(16.3725042, 48.2083537),
            ProcessingDelayMins = 186
        };
        
        private static readonly Transferwarehouse ValidTransferWarehouse02 = new Transferwarehouse
        {
            Code = "TRAN02",
            HopType = HopType.TransferWarehouse,
            Description = "TransferWarehouse - Location02",
            LocationName = "Location02",
            LocationCoordinates = new Point(16.3725042, 48.2083537),
            ProcessingDelayMins = 186
        };
        
        private readonly Warehouse _validWarehouse = new Warehouse
        {
            Code = "AUTA05",
            HopType = HopType.Warehouse,
            Description = "Root Warehouse - Österreich",
            LocationName = "Root",
            Level = 0,
            LocationCoordinates = new Point(16.3725042, 48.2083537),
            ProcessingDelayMins = 186,
            NextHops = new List<Hop>()
            {
                new Warehouse
                {
                    Code = "WENA04",
                    HopType = HopType.Warehouse,
                    Description = "Warehouse Level 4 - Wien",
                    LocationName = "Wien",
                    Level = 4,
                    LocationCoordinates = new Point(16.3725042, 48.2083537),
                    ProcessingDelayMins = 160,
                },
                ValidTransferWarehouse01,
                ValidTransferWarehouse02
            }
        };
        
        [SetUp]
        public void Setup()
        {
            _entities = new List<Hop>()
            {
                _validWarehouse,
                ValidTransferWarehouse01,
                ValidTransferWarehouse02
            };

            var dbMock = new Mock<ITechnikumDirektContext>();
            dbMock.Setup(p => p.Hops).Returns(DbContextMock.GetQueryableMockDbSet<Hop>(_entities));
            
            dbMock.Setup(p => p.SaveChanges()).Returns(1);
            
            dbMock.Setup(p => p.Warehouses.Find(It.IsAny<object[]>()))
                .Returns<object[]>((keyValues) =>
                    (Warehouse) _entities.FirstOrDefault(y => y.Code == (string) keyValues.GetValue(0)));

            _technikumDirektContext = dbMock.Object;
            _logger = NullLogger<WarehouseRepository>.Instance;
        }

        #region GetAll

        [Test]
        public void GetAll_ReturnsWarehouseStructure_ValidWarehouseStructure()
        {
            _warehouseRepository = new WarehouseRepository(_technikumDirektContext, _logger);
            var wh = _warehouseRepository.GetAll();
            Assert.NotNull(wh);
            Assert.IsInstanceOf<Warehouse>(wh.FirstOrDefault());
            Assert.AreSame(_validWarehouse, wh.FirstOrDefault());
        }

        #endregion
        
        #region GetTransferWarehouses

        [Test]
        public void GetTransferWarehouses_ReturnsTransferWarehouses_ValidWarehouseStructure()
        {
            _warehouseRepository = new WarehouseRepository(_technikumDirektContext, _logger);
            var wh = _warehouseRepository.GetTransferWarehouses();
            Assert.NotNull(wh);
            Assert.IsInstanceOf<Transferwarehouse>(wh.FirstOrDefault());
            Assert.AreSame(ValidTransferWarehouse01, wh.FirstOrDefault());
        }

        #endregion

        #region GetWarehouseByCode

        [Test]
        public void GetWarehouseByCode_ReturnsValidWarehouse_ValidHopCode()
        {
            _warehouseRepository = new WarehouseRepository(_technikumDirektContext, _logger);
            var wh = _warehouseRepository.GetWarehouseByCode(ValidHopCode);
            Assert.NotNull(wh);
            Assert.IsInstanceOf<Warehouse>(wh);
        }

        [Test]
        public void GetWarehouseByCode_Throws_InValidHopCode()
        {
            _warehouseRepository = new WarehouseRepository(_technikumDirektContext, _logger);
            Assert.Throws<DataAccessNotFoundException>(
                () => _warehouseRepository.GetWarehouseByCode(InvalidHopCode));
        }

        #endregion

        #region ImportWarehouse

        [Test]
        public void ImportWarehouses_Throws_InValidHopCode()
        {
            _warehouseRepository = new WarehouseRepository(_technikumDirektContext, _logger);
            Assert.DoesNotThrow(() => _warehouseRepository.ImportWarehouses(_validWarehouse));
        }

        #endregion
    }
}