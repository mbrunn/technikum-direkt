using System.Collections.Generic;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Interfaces
{
    public interface ITransferwarehouseRepository : ISearchableRepository<Transferwarehouse>
    {
        /*
        IEnumerable<Transferwarehouse> GetAllTransferwarehousesOfLogisticsPartner(string LogisticsPartnerName);
        */
    }
}