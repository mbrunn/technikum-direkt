using System.Collections.Generic;
using AutoMapper;
using FluentValidation;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.BusinessLogic.Models;
using TechnikumDirekt.DataAccess.Interfaces;
using Warehouse = TechnikumDirekt.BusinessLogic.Models.Warehouse;
using DalModels = TechnikumDirekt.DataAccess.Models;

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

        public Warehouse ExportWarehouses()
        {
            var dalHops = _warehouseRepository.GetAll();
            
            DalModels.Warehouse rootWarehouse = null;

            foreach (var wh in dalHops)
            {
                if (wh is DataAccess.Models.Warehouse warehouse && warehouse.Level == 0) rootWarehouse = warehouse;
            }

            var blWarehouse = _mapper.Map<Warehouse>(rootWarehouse);
            return blWarehouse;
        }

        public Warehouse GetWarehouse(string code)
        {
            _hopValidator.Validate(new Hop {Code = code}, 
                options =>
                {
                    options.IncludeRuleSets("code");
                    options.ThrowOnFailures();
                });

            var dalWarehouse = _warehouseRepository.GetWarehouseByCode(code);
            if (dalWarehouse == null)
            {
                throw new TrackingLogicException("Hüfe, i hob kan Code gfunden!"); //TODO: DO NOT CHANGE
            }
            
            var blWarehouse = _mapper.Map<Warehouse>(dalWarehouse);
            return blWarehouse;
        }

        public void ImportWarehouses(Warehouse warehouse)
        {
            ValidateWarehouseTree(warehouse);
            _warehouseRepository.ClearWarehouses();
            var dalWh = _mapper.Map<DataAccess.Models.Warehouse>(warehouse);
            _warehouseRepository.ImportWarehouses(dalWh);
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