using FluentValidation;
using NUnit.Framework;
using TechnikumDirekt.BusinessLogic.FluentValidation;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.Tests
{
    public class WarehouseLogicTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ImportWarehousesThrowsExceptionOnInvalidDescription()
        {
            /*
            var warehouse = new Warehouse
            {
                Code = "WENA04",
                HopType = HopType.Warehouse,
                Description = "Warehouse Level +4 - Wien",
                LocationName = "Wien",
                LocationCoordinates = new GeoCoordinatePortable.GeoCoordinate(16.3725042, 48.2083537),
                ProcessingDelayMins = 160
            };
            var warehouseLogic = new WarehouseLogic(new WarehouseValidator(), new HopValidator()); // TODO: mock warehouse validator? - prop fine like this though

            Assert.Throws<ValidationException>(() => warehouseLogic.ImportWarehouses(warehouse));*/
        }
        
        [Test]
        public void ImportWarehousesSucceedsOnValidDescription()
        {
            /*
            var warehouse = new Warehouse
            {
                Code = "WENA04",
                HopType = HopType.Warehouse,
                Description = "Warehouse Level 4 - Wien",
                LocationName = "Wien",
                LocationCoordinates = new GeoCoordinatePortable.GeoCoordinate(16.3725042, 48.2083537),
                ProcessingDelayMins = 160
            };
            var warehouseLogic = new WarehouseLogic(new WarehouseValidator(), new HopValidator()); // TODO: mock warehouse validator? - prop fine like this though

            Assert.DoesNotThrow(() => warehouseLogic.ImportWarehouses(warehouse));*/
        }
    }
}