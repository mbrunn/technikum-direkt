using System;
using System.Collections.Generic;
using System.Linq;
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
            _logger.LogDebug($"Exported new Warehousestructure");
            return blWarehouse;
        }

        public Hop GetWarehouse(string code)
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
                _logger.LogDebug($"Validation of Hop with Hopcode {code} failed.");
                throw new BusinessLogicValidationException("Hop validation failed.", e);
            }

            DalModels.Hop dalHop;
            try
            {
                dalHop = _hopRepository.GetHopByCode(code);
            }
            catch (DataAccessNotFoundException e)
            {
                _logger.LogTrace("Hüfe, i hob kan Code gfunden!");
                throw new BusinessLogicNotFoundException("Hüfe, i hob kan Code gfunden!", e); //DO NOT CHANGE
            }

            var blHop = _mapper.Map<Hop>(dalHop);
            _logger.LogDebug($"Found warehouse with hopcode {code}.");
            return blHop;
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
            try
            {
                _warehouseRepository.ImportWarehouses(dalWh);
            }
            catch (Exception e)
            {
                throw;
            }
            _logger.LogDebug($"Importet warehouse with hopcode {warehouse.Code}");
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
                    _hopValidator.ValidateAndThrow(node);
                    break;
                case HopType.TransferWarehouse:
                    //_hopValidator.ValidateAndThrow(node);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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

        public IEnumerable<Transferwarehouse> GetTransferWarehouses()
        {
            var dalHops = _warehouseRepository.GetTransferWarehouses();

            if (dalHops == null)
            {
                _logger.LogTrace($"No TransferWarehouses imported.");
                    throw new BusinessLogicNotFoundException("No TransferWarehouses imported.");
            }
            
            if (!dalHops.Any())
            {
                _logger.LogTrace($"No TransferWarehouses imported.");
                throw new BusinessLogicNotFoundException("No TransferWarehouses imported.");
            }

            var blTransferWarehouses = _mapper.Map<List<Transferwarehouse>>(dalHops);
            _logger.LogDebug($"Exported {blTransferWarehouses.Count()} TransferWarehouses");
            return blTransferWarehouses;
        }
    }
}