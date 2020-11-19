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
            var wh = _dbContext.Hops.ToList();
            _logger.LogTrace($"Read" + wh.Count + " out of the repository");
            return wh;
        }

        public Warehouse GetWarehouseByCode(string code)
        {
            if (string.IsNullOrEmpty(code)) throw new DataAccessArgumentNullException("Code is null.");
            
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

        public void ImportWarehouses(Warehouse warehouse)
        {
            _dbContext.Warehouses.Add(warehouse);
            _dbContext.SaveChanges();
            _logger.LogTrace($"Imported warehouse with hopCode {warehouse.Code}.");
        }

        public void ClearWarehouses()
        {
            _dbContext.Database.ExecuteSqlRaw(
                $"DELETE FROM {_dbContext.Model.FindEntityType(typeof(Parcel)).GetTableName()}");
            _dbContext.Database.ExecuteSqlRaw(
                $"DELETE FROM {_dbContext.Model.FindEntityType(typeof(Recipient)).GetTableName()}");
            _dbContext.Database.ExecuteSqlRaw(
                $"DELETE FROM {_dbContext.Model.FindEntityType(typeof(Hop)).GetTableName()}");
            _logger.LogTrace($"Cleared Warehousestructure and all other data.");
        }
    }
}