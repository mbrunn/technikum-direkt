using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class HopRepositoryTests
    {
        private ITechnikumDirektContext _technikumDirektContext;
        private IHopRepository _hopRepository;
        private List<Hop> _entities;
        private NullLogger<HopRepository> _logger;

        private const string ValidHopCode = "ABCD1234";
        private const string InvalidHopCode = "AbdA2a";
        
        private readonly Point _validPoint = new Point(42.0, 42.0);
        private readonly Point _inValidPoint = new Point(25.0, 25.0);

        [SetUp]
        public void Setup()
        {
            var validHop = new Hop()
            {
                HopType = HopType.Warehouse,
                Code = ValidHopCode,
                Description = "Valid Truck in Siebenhirten",
                LocationName = "Siebenhirten",
                LocationCoordinates = new Point(48.129885504996, 16.3111537799009),
                ProcessingDelayMins = 231,
            };

            _entities = new List<Hop>()
            {
                validHop
            };

            var dbMock = new Mock<ITechnikumDirektContext>();
            dbMock.Setup(p => p.Hops).Returns(DbContextMock.GetQueryableMockDbSet<Hop>(_entities));
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
        public void GetHopContainingPoint_ReturnsValidHop_WithValidPoint()
        {
            _hopRepository = new HopRepository(_technikumDirektContext, _logger);
            var entity = _hopRepository.GetHopContainingPoint(_validPoint);
            Assert.NotNull(entity);
            Assert.AreSame(_entities.FirstOrDefault(), entity);
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