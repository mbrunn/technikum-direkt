using System.Collections.Generic;
using System.Linq;
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
        
        public IEnumerable<Hop> GetAll()
        {
            var wh = _dbContext.Hops.ToList();
            return wh;
        }

        public Warehouse GetWarehouseByCode(string code)
        { 
            return _dbContext.Warehouses.Find(code);
        }

        public void ImportWarehouses(Warehouse warehouse)
        {
            _dbContext.Warehouses.Add(warehouse);
            _dbContext.SaveChanges();
        }

        public void ClearWarehouses()
        {
            _dbContext.Database.ExecuteSqlRaw(
                $"DELETE FROM {_dbContext.Model.FindEntityType(typeof(Hop)).GetTableName()}");
        }

        public IEnumerable<Warehouse> GetWarehousesOnLevel(int level)
        {
            throw new System.NotImplementedException();
        }

        private void AddParentWarehouseCode(Hop hop)
        {
           
        }
    }
}