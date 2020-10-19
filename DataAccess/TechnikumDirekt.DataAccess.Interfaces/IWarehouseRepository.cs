using System.Collections;
using System.Collections.Generic;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Interfaces
{
    public interface IWarehouseRepository : ISearchableRepository<Warehouse>
    {
        Warehouse GetWarehouseWithCode(string code);
        IEnumerable<Warehouse> GetWarehousesOnLevel(int level);
    }
}