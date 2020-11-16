using System;
using System.Collections.Generic;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
using TechnikumDirekt.BusinessLogic.Exceptions;
using TechnikumDirekt.BusinessLogic.Interfaces;
using TechnikumDirekt.BusinessLogic.Models;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Sql.Exceptions;
using DalModels = TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.BusinessLogic
{
    public class WarehouseLogic : IWarehouseLogic
    {
        private readonly IValidator<Warehouse> _warehouseValidator;
        private readonly IValidator<Hop> _hopValidator;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IHopRepository _hopRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<WarehouseLogic> _logger;

        public WarehouseLogic(IValidator<Warehouse> warehouseValidator, IValidator<Hop> hopValidator,
            IWarehouseRepository warehouseRepository, IHopRepository hopRepository,
            IMapper mapper, ILogger<WarehouseLogic> logger)
        {
            _warehouseValidator = warehouseValidator;
            _hopValidator = hopValidator;
            _warehouseRepository = warehouseRepository;
            _hopRepository = hopRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public Warehouse ExportWarehouses()
        {
            var dalHops = _warehouseRepository.GetAll();

            DalModels.Warehouse rootWarehouse = null;

            foreach (var wh in dalHops)
            {
                if (wh is DalModels.Warehouse warehouse && warehouse.Level == 0) rootWarehouse = warehouse;
            }

            if (rootWarehouse == null)
            {
                _logger.LogTrace($"No warehouses imported.");
                throw new BusinessLogicNotFoundException("No warehouses imported.");
            }

            var blWarehouse = _mapper.Map<Warehouse>(rootWarehouse);
            _logger.LogDebug($"Imported new Warehousestructure");
            return blWarehouse;
        }

        public Warehouse GetWarehouse(string code)
        {
            try
            {
                _hopValidator.Validate(new Hop {Code = code},
                    options =>
                    {
                        options.IncludeRuleSets("code");
                        options.ThrowOnFailures();
                    });
            }
            catch (ValidationException e)
            {
                throw new BusinessLogicValidationException("Hop validation failed.", e);
            }

            DalModels.Warehouse dalWarehouse;
            try
            {
                dalWarehouse = _warehouseRepository.GetWarehouseByCode(code);
            }
            catch (DataAccessNotFoundException e)
            {
                _logger.LogTrace("Hüfe, i hob kan Code gfunden!");
                throw new BusinessLogicNotFoundException("Hüfe, i hob kan Code gfunden!", e); //DO NOT CHANGE
            }

            var blWarehouse = _mapper.Map<Warehouse>(dalWarehouse);
            _logger.LogDebug($"Found warehouse with hopcode {code}.");
            return blWarehouse;
        }

        public void ImportWarehouses(Warehouse warehouse)
        {
            try
            {
                ValidateWarehouseTree(warehouse);
            }
            catch (ValidationException e)
            {
                throw new BusinessLogicValidationException("Warehouse tree validation failed.", e);
            }
            _warehouseRepository.ClearWarehouses();
            var dalWh = _mapper.Map<DalModels.Warehouse>(warehouse);
            _warehouseRepository.ImportWarehouses(dalWh);
            _logger.LogDebug($"Imporeted warehouse with hopcode {warehouse.Code}");
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

        public Hop GetHopContainingPoint(Point point)
        {
            var hop = _hopRepository.GetHopContainingPoint(point);
            if (hop == null)
            {
                throw new BusinessLogicNotFoundException($"Hop containing the point {point.Coordinate} couldn't be found.");
            }

            return _mapper.Map<Hop>(hop);
        }
    }
}