using System.Collections.Generic;
using System.Linq;
using Moq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using TechnikumDirekt.DataAccess.Models;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Sql;

namespace TechnikumDirekt.DataAccess.Tests
{
    public class HopRepositoryTests
    {
        private ITechnikumDirektContext  _technikumDirektContext;
        private IHopRepository _hopRepository;
        private List<Hop> _entities;
        
        private const string ValidHopCode = "ABCD1234";
        private const string InvalidHopCode = "AbdA2a";
        
        [SetUp]
        public void Setup()
        {
            var validHop = new Hop()
            {
                HopType = HopType.Truck,
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
        }
        
        [Test]
        public void GetHopByCode_ReturnsValidHop_WithValidHopCode()
        {
            _hopRepository = new HopRepository(_technikumDirektContext);
            var entity = _hopRepository.GetHopByCode(ValidHopCode);
            Assert.NotNull(entity);
            Assert.AreSame(_entities.FirstOrDefault(), entity);
        }
        
        [Test]
        public void GetHopByCode_ReturnsNull_WithInValidHopCode()
        {
            _hopRepository = new HopRepository(_technikumDirektContext);
            var entity = _hopRepository.GetHopByCode(InvalidHopCode);
            Assert.Null(entity);
        }
        
        [Test]
        public void GetHopByCode_ReturnsNull_WithNullHopCode()
        {
            _hopRepository = new HopRepository(_technikumDirektContext);
            var entity = _hopRepository.GetHopByCode(null);
            Assert.Null(entity);
        }
    }
}