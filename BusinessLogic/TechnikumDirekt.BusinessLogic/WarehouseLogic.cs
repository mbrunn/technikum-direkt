using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.BusinessLogic.Models;
using Warehouse = TechnikumDirekt.BusinessLogic.Models.Warehouse;

namespace TechnikumDirekt.BusinessLogic
{
    public class WarehouseLogic : IWarehouseLogic
    {
        public static readonly List<Warehouse> Warehouses = new List<Warehouse>
        {
            /*new Warehouse
            {
                Code = "WENA04",
                HopType = HopType.Warehouse,
                Description = "Warehouse Level 4 - Wien",
                LocationName = "Wien",
                Level = 4,
                LocationCoordinates = new GeoCoordinatePortable.GeoCoordinate(16.3725042, 48.2083537),
                ProcessingDelayMins = 160
            },
            new Warehouse
            {
                Code = "WENA03",
                HopType = HopType.Warehouse,
                Description = "Warehouse Level 3 - Wien",
                LocationName = "Wien",
                Level = 3,
                LocationCoordinates = new GeoCoordinatePortable.GeoCoordinate(16.3725042, 48.2083537),
                ProcessingDelayMins = 299
            }*/
        };

        private readonly IValidator<Warehouse> _warehouseValidator;
        private readonly IValidator<Hop> _hopValidator;

        public WarehouseLogic(IValidator<Warehouse> warehouseValidator, IValidator<Hop> hopValidator)
        {
            _warehouseValidator = warehouseValidator;
            _hopValidator = hopValidator;
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
            _warehouseValidator.ValidateAndThrow(warehouse);
            ValidateWarehouseTree(warehouse);

            Warehouses.Add(warehouse);
        }

        private void ValidateWarehouseTree(Hop node)
        {
            try
            {
                switch (node.HopType)
                {
                    case HopType.Warehouse:
                        _warehouseValidator.ValidateAndThrow((Warehouse) node);
                        var whHelper = (Warehouse) node;
                        foreach (var child in whHelper.NextHops)
                        {
                            ValidateWarehouseTree(child.Hop);
                        }
                        break;
                    case HopType.Truck:
                    case HopType.TransferWarehouse:
                        _hopValidator.ValidateAndThrow(node);
                        break;
                }
            }
            catch (ValidationException)
            {
                Console.WriteLine($"Invalid object: {node.Description}");
            }
        }
    }
}