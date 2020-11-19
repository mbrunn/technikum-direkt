using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
using NUnit.Framework;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;
using TechnikumDirekt.DataAccess.Sql;
using TechnikumDirekt.DataAccess.Sql.Exceptions;
using Assert = NUnit.Framework.Assert;

namespace TechnikumDirekt.DataAccess.Tests
{
    public class HopRepositoryTests
    {
        private ITechnikumDirektContext _technikumDirektContext;
        private IHopRepository _hopRepository;
        private List<Hop> _entities;
        private List<Truck> _truckEntities;
        private List<Transferwarehouse> _transferwarehouseEntities;
        
        private NullLogger<HopRepository> _logger;

        private const string ValidHopCode = "ABCD1234";
        private const string InvalidHopCode = "AbdA2a";
        
        private readonly Point _validTruckPoint = new Point(42.0, 42.0);
        private readonly Point _validTransferwarehousePoint = new Point(150, 150);
        private readonly Point _inValidPoint = new Point(250, 250);

        [SetUp]
        public void Setup()
        {
            var validHop = new Hop()
            {
                HopType = HopType.Warehouse,
                Code = ValidHopCode,
                Description = "Valid Truck in Siebenhirten",
                LocationName = "Siebenhirten",
                LocationCoordinates = _validTruckPoint,
                ProcessingDelayMins = 231,
            };

            var truckRegionGeometry = new Polygon( new LinearRing(new []
            {
                new Coordinate(0, 0),
                new Coordinate(0, 50),
                new Coordinate(50, 50),
                new Coordinate(50, 0),
                new Coordinate(0, 0)
            }));
            
            var transferwarehosueRegionGeometry = new Polygon( new LinearRing(new []
            {
                new Coordinate(60, 60),
                new Coordinate(60, 160),
                new Coordinate(160, 160),
                new Coordinate(160, 60),
                new Coordinate(60, 60)
            }));
            
            var truck = new Truck()
            {
                Code = "ABCD1234",
                Description = "TruckDescription",
                HopArrivals = null,
                HopType = HopType.Truck,
                LocationCoordinates = _validTruckPoint,
                LocationName = "Benjis City",
                NumberPlate = "MI-12354",
                ParentTraveltimeMins = 10,
                ParentWarehouse = null,
                ParentWarehouseCode = null,
                RegionGeometry = truckRegionGeometry
            };
            
            var transferwarehouse = new Transferwarehouse()
            {
                Code = "TranferCode",
                Description = "TestTransferWh",
                HopArrivals = null,
                HopType = HopType.TransferWarehouse,
                LocationCoordinates = _validTransferwarehousePoint,
                LocationName = "transferLocation",
                LogisticsPartner = "Yeetmann Gruppe",
                LogisticsPartnerUrl = "www.123.at",
                ParentWarehouse = null,
                RegionGeometry = _validTransferwarehousePoint
            };

            _entities = new List<Hop>()
            {
                validHop
            };
            
            _truckEntities = new List<Truck>()
            {
                truck
            };
            
            _transferwarehouseEntities = new List<Transferwarehouse>()
            {
                transferwarehouse
            };

            var dbMock = new Mock<ITechnikumDirektContext>();
            dbMock.Setup(p => p.Hops).Returns(DbContextMock.GetQueryableMockDbSet<Hop>(_entities));
            
            dbMock.Setup(p => p.Trucks).Returns(DbContextMock.GetQueryableMockDbSet<Truck>(_truckEntities));
            dbMock.Setup(p => p.Transferwarehouses)
                .Returns(DbContextMock.GetQueryableMockDbSet<Transferwarehouse>(_transferwarehouseEntities));
            
            dbMock.Setup(p => p.SaveChanges()).Returns(1);

            dbMock.Setup(p => p.Hops.Find(It.IsAny<object[]>()))
                .Returns<object[]>((keyValues) =>
                    _entities.FirstOrDefault(y => y.Code == (string) keyValues.GetValue(0)));

            _technikumDirektContext = dbMock.Object;
            _logger = NullLogger<HopRepository>.Instance;
        }

        #region GetHopByCode

        [Test]
        public void GetHopByCode_ReturnsValidHop_WithValidHopCode()
        {
            _hopRepository = new HopRepository(_technikumDirektContext, _logger);
            var entity = _hopRepository.GetHopByCode(ValidHopCode);
            Assert.NotNull(entity);
            Assert.AreSame(_entities.FirstOrDefault(), entity);
        }

        [Test]
        public void GetHopByCode_Throws_WithInValidHopCode()
        {
            _hopRepository = new HopRepository(_technikumDirektContext, _logger);
            Assert.Throws<DataAccessNotFoundException>(() => _hopRepository.GetHopByCode(InvalidHopCode));
        }

        [Test]
        public void GetHopByCode_Throws_WithNullHopCode()
        {
            _hopRepository = new HopRepository(_technikumDirektContext, _logger);
            Assert.Throws<DataAccessArgumentNullException>(() => _hopRepository.GetHopByCode(null));
        }

        #endregion
        
        #region GetHopContainingPoint

        [Test]
        public void GetHopContainingPoint_ReturnsValidTruck_WithValidTruckPoint()
        {
            _hopRepository = new HopRepository(_technikumDirektContext, _logger);
            var entity = _hopRepository.GetHopContainingPoint(_validTruckPoint);
            
            Assert.NotNull(entity);
            Assert.AreSame(_truckEntities.FirstOrDefault(), entity);
        }
        
        [Test]
        public void GetHopContainingPoint_ReturnsValidTransferwarehouse_WithValidTransferwarehousePoint()
        {
            _hopRepository = new HopRepository(_technikumDirektContext, _logger);
            var entity = _hopRepository.GetHopContainingPoint(_validTransferwarehousePoint);
            
            Assert.NotNull(entity);
            Assert.AreSame(_transferwarehouseEntities.FirstOrDefault(), entity);
        }

        [Test]
        public void GetHopContainingPoint_Throws_WithInValidPoint()
        {
            _hopRepository = new HopRepository(_technikumDirektContext, _logger);
            Assert.Throws<DataAccessNotFoundException>(() => _hopRepository.GetHopContainingPoint(_inValidPoint));
        }

        [Test]
        public void GetHopContainingPoint_Throws_WithNullPoint()
        {
            _hopRepository = new HopRepository(_technikumDirektContext, _logger);
            Assert.Throws<DataAccessArgumentNullException>(() => _hopRepository.GetHopContainingPoint(null));
        }

        #endregion
    }
}