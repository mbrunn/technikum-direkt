using System.Collections.Generic;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Interfaces
{
    public interface IWarehouseRepository
    {
        IEnumerable<Hop> GetAll();
        Warehouse GetWarehouseByCode(string code);
        void ImportWarehouses(Warehouse warehouse);
        void ClearWarehouses();
    }
}