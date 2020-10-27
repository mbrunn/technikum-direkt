using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentValidation;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.BusinessLogic.Models;
using TechnikumDirekt.DataAccess.Interfaces;
using Warehouse = TechnikumDirekt.BusinessLogic.Models.Warehouse;

namespace TechnikumDirekt.BusinessLogic
{
    public class WarehouseLogic : IWarehouseLogic
    {
        public static List<Warehouse> Warehouses = new List<Warehouse>();

        private readonly IValidator<Warehouse> _warehouseValidator;
        private readonly IValidator<Hop> _hopValidator;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IMapper _mapper;

        public WarehouseLogic(IValidator<Warehouse> warehouseValidator, IValidator<Hop> hopValidator, IWarehouseRepository warehouseRepository, 
            IMapper mapper)
        {
            _warehouseValidator = warehouseValidator;
            _hopValidator = hopValidator;
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
        }

        public IEnumerable<Warehouse> ExportWarehouses()
        {
            if (Warehouses.Count == 0) throw new TrackingLogicException(); // TODO - use notfound exception
            
            return Warehouses;
        }

        public Warehouse GetWarehouse(string code)
        {
            _hopValidator.Validate(new Hop {Code = code}, 
                options =>
                {
                    options.IncludeRuleSets("code");
                    options.ThrowOnFailures();
                });
            return Warehouses.FirstOrDefault(w => w.Code == code);
        }

        public void ImportWarehouses(Warehouse warehouse)
        {
            ValidateWarehouseTree(warehouse);
            _warehouseRepository.ClearWarehouses();
            _warehouseRepository.ImportWarehouses(_mapper.Map<DataAccess.Models.Warehouse>(warehouse));
        }
        
        private void ValidateWarehouseTree(Hop node)
        {
            switch (node.HopType)
            {
                case HopType.Warehouse:
                    _warehouseValidator.ValidateAndThrow((Warehouse) node);
                    var whHelper = (Warehouse) node;
                    if (whHelper.NextHops == null) break;
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
    }
}