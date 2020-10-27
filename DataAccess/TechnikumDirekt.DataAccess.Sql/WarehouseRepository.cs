using System.Collections.Generic;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Sql
{
    public class WarehouseRepository : IWarehouseRepository
    {
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
            throw new System.NotImplementedException();
        }

        public IEnumerable<Warehouse> GetWarehousesOnLevel(int level)
        {
            throw new System.NotImplementedException();
        }
    }
}