using System.Collections;
using System.Collections.Generic;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Interfaces
{
    public interface IWarehouseRepository
    {
        IEnumerable<Warehouse> GetAll();
        Warehouse GetWarehouseByCode(string code);
        void ImportWarehouses(Warehouse warehouse);
        IEnumerable<Warehouse> GetWarehousesOnLevel(int level);
    }
}