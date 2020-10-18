using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NUnit.Framework;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.FluentValidation;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.Tests
{
    public class WarehouseLogicTests
    {
        private readonly Warehouse _validWarehouse = new Warehouse
        {
            Code = "WENA04",
            HopType = HopType.Warehouse,
            Description = "Warehouse Level 4 - Wien",
            LocationName = "Wien",
            Level = 4,
            LocationCoordinates = new GeoCoordinatePortable.GeoCoordinate(16.3725042, 48.2083537),
            ProcessingDelayMins = 160
        };
        
        private readonly Warehouse _invalidWarehouse = new Warehouse
        {
            Code = "WENA04",
            HopType = HopType.Warehouse,
            Description = "Warehouse Level* 4 - Wien",
            LocationName = "Wien",
            Level = 4,
            LocationCoordinates = new GeoCoordinatePortable.GeoCoordinate(16.3725042, 48.2083537),
            ProcessingDelayMins = 160
        };
        
        private const string ValidHopCode = "ABCD1234";
        private const string InvalidHopCode = "AbdA2a";

        #region ExportWarehouses Tests

        [Test]
        public void ExportWarehouses_Throws_WithEmptyWarehouseList()
        {
            var wl = new WarehouseLogic(new WarehouseValidator(), new HopValidator());
            WarehouseLogic.Warehouses = new List<Warehouse>();

            Assert.Throws<TrackingLogicException>(() => wl.ExportWarehouses());
        }
        
        [Test]
        public void ExportWarehouses_DoesNotThrowAndReturnsData_WithNonEmptyWarehouseList()
        {
            var wl = new WarehouseLogic(new WarehouseValidator(), new HopValidator());

            wl.ImportWarehouses(_validWarehouse);
            
            List<Warehouse> warehouses = null;
            Assert.DoesNotThrow(() => warehouses = wl.ExportWarehouses()?.ToList());
            
            Assert.IsNotNull(warehouses);
            Assert.Greater(warehouses.Count, 0);
        }

        #endregion

        #region ImportWarehouses Tests

        [Test]
        public void ImportWarehouses_DoesNotThrow_WithValidWarehouse()
        {
            var wl = new WarehouseLogic(new WarehouseValidator(), new HopValidator());

            Assert.DoesNotThrow(() => wl.ImportWarehouses(_validWarehouse));
        }
        
        [Test]
        public void ImportWarehouses_Throws_WithInvalidWarehouse()
        {
            var wl = new WarehouseLogic(new WarehouseValidator(), new HopValidator());

            Assert.Throws<ValidationException>(() => wl.ImportWarehouses(_invalidWarehouse));
        }

        #endregion

        #region GetWarehouse Tests

        [Test]
        public void GetWarehouse_DoesNotThrow_OnValidHopCode()
        {
            var wl = new WarehouseLogic(new WarehouseValidator(), new HopValidator());
            
            Assert.DoesNotThrow(() => wl.GetWarehouse(ValidHopCode));
        }
        
        [Test]
        public void GetWarehouse_Throws_OnInvalidHopCode()
        {
            var wl = new WarehouseLogic(new WarehouseValidator(), new HopValidator());
            
            Assert.Throws<ValidationException>(() => wl.GetWarehouse(InvalidHopCode));
        }

        #endregion
    }
}