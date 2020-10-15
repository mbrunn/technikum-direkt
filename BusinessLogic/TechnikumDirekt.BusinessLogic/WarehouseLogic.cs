using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.BusinessLogic.Models;
using Warehouse = TechnikumDirekt.BusinessLogic.Models.Warehouse;

namespace TechnikumDirekt.BusinessLogic
{
    public class WarehouseLogic: IWarehouseLogic
    {
        public static readonly List<Warehouse> Warehouses = new List<Warehouse>
        {
            new Warehouse
            {
                Code = "WENA04",
                HopType = HopType.Warehouse,
                Description = "Warehouse Level 4 - Wien",
                LocationName = "Wien",
                LocationCoordinates = new GeoCoordinatePortable.GeoCoordinate(16.3725042, 48.2083537),
                ProcessingDelayMins = 160
            },
            new Warehouse
            {
                Code = "WENA03",
                HopType = HopType.Warehouse,
                Description = "Warehouse Level 3 - Wien",
                LocationName = "Wien",
                LocationCoordinates = new GeoCoordinatePortable.GeoCoordinate(16.3725042, 48.2083537),
                ProcessingDelayMins = 299
            }
        };

        private readonly IValidator<Warehouse> _validator;
        
        public WarehouseLogic(IValidator<Warehouse> validator)
        {
            _validator = validator;
        }
        
        public IEnumerable<Warehouse> ExportWarehouses()
        {
            return Warehouses;
        }

        public Warehouse GetWarehouse(string code)
        {
            return Warehouses.FirstOrDefault(w => w.Code == code);
        }

        public void ImportWarehouses(Warehouse warehouse)
        {
            _validator.ValidateAndThrow(warehouse);
            
            Warehouses.Add(warehouse);
        }
    }
}