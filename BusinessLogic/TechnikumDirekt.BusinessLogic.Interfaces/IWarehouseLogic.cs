using System.Collections.Generic;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.Interfaces
{
    public interface IWarehouseLogic
    {
        /// <summary> Returns the list of registered warehouses. </summary>
        public IEnumerable<Warehouse> ExportWarehouses();
        /// <summary> Returns a single warehouse. </summary>
        /// <param name="code">Code of the warehouse to return</param>
        public Warehouse GetWarehouse(string code);
        /// <summary> Imports a list of warehouses into the system. </summary>
        /// <param name="warehouses">Warehouses to import</param>
        public void ImportWarehouses(Warehouse warehouse);
    }
}