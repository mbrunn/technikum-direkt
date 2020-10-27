using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Sql
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly TechnikumDirektContext _dbContext;
        
        public WarehouseRepository(TechnikumDirektContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public IEnumerable<Warehouse> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public Warehouse GetWarehouseByCode(string code)
        {
            throw new System.NotImplementedException();
        }

        public void ImportWarehouses(Warehouse warehouse)
        {
            _dbContext.Warehouses.Add(warehouse);
        }

        public void ClearWarehouses()
        {
            /*_dbContext.Database.ExecuteSqlRaw(
                $"TRUNCATE TABLE {_dbContext.Model.FindEntityType(typeof(Hop)).GetTableName()}");*/
        }

        public IEnumerable<Warehouse> GetWarehousesOnLevel(int level)
        {
            throw new System.NotImplementedException();
        }
    }
}