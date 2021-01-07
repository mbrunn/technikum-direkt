using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;
using TechnikumDirekt.DataAccess.Sql.Exceptions;

namespace TechnikumDirekt.DataAccess.Sql
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly ITechnikumDirektContext _dbContext;
        private readonly ILogger<WarehouseRepository> _logger;

        public WarehouseRepository(ITechnikumDirektContext dbContext, ILogger<WarehouseRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IEnumerable<Hop> GetAll()
        {
            try
            {
                var wh = _dbContext.Hops.ToList();
                _logger.LogTrace($"Read" + wh.Count + " Warehouses out of the repository");
                return wh;
            }
            catch (Exception e)
            {
                _logger.LogTrace($"Could not read Warehousestructure.");
                throw new DataAccessNotFoundException();
            }
        }
        
        public Warehouse GetWarehouseByCode(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    throw new DataAccessArgumentNullException("Code can't be null for GetWarehouseByCode");

                var wh = _dbContext.Warehouses.Find(code);

                if (wh != null)
                {
                    _logger.LogTrace($"Hop with code {code} has been found.");
                }
                else
                {
                    _logger.LogTrace($"Hop with code {code} couldn't be found.");
                    throw new DataAccessNotFoundException($"Hop with code {code} couldn't be found.");
                }

                return wh;
            }
            catch (Exception e)
            {
                _logger.LogTrace($"Hop with code {code} couldn't be found.");
                throw new DataAccessNotFoundException();
            }
        }

        public void ImportWarehouses(Warehouse warehouse)
        {
            try
            {
                _dbContext.Warehouses.Add(warehouse);
                _dbContext.SaveChanges();
                _logger.LogTrace($"Imported warehouse with hopCode {warehouse.Code}.");
            }
            catch (Exception e)
            {
                _logger.LogTrace($"Warehousestructure couldn't be added.");
                throw new DataAccessAddException();
            }
        }

        public void ClearWarehouses()
        {
            try
            {
                _dbContext.Database.ExecuteSqlRaw(
                    $"DELETE FROM {_dbContext.Model.FindEntityType(typeof(Webhook)).GetTableName()}");
                _dbContext.Database.ExecuteSqlRaw(
                    $"DELETE FROM {_dbContext.Model.FindEntityType(typeof(Parcel)).GetTableName()}");
                _dbContext.Database.ExecuteSqlRaw(
                    $"DELETE FROM {_dbContext.Model.FindEntityType(typeof(Recipient)).GetTableName()}");
                _dbContext.Database.ExecuteSqlRaw(
                    $"DELETE FROM {_dbContext.Model.FindEntityType(typeof(Hop)).GetTableName()}");
                _logger.LogTrace($"Cleared Warehousestructure and all other data.");
            }
            catch (Exception e)
            {
                _logger.LogTrace($"Warehousestructure couldn't be cleared.");
                throw new DataAccessNotPossibleException();
            }
        }

        public IEnumerable<Hop> GetTransferWarehouses()
        {
            try
            {
                var transferWarehouses = _dbContext.Hops.Where(hop => hop.HopType == HopType.TransferWarehouse).ToList();
                _logger.LogTrace($"Read" + transferWarehouses.Count + " TransferWarehouses out of the repository");
                return transferWarehouses;
            }
            catch (Exception e)
            {
                _logger.LogTrace($"No Transferwarehouse found.");
                throw new DataAccessNotFoundException();
            }
        }
    }
}