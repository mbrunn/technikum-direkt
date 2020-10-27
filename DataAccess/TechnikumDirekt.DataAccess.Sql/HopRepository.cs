using System.Threading.Tasks;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Sql
{
    public class HopRepository : IHopRepository
    {
        private readonly TechnikumDirektContext _dbContext;
        
        public HopRepository(TechnikumDirektContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public Hop GetHopByCode(string hopCode)
        {
            return _dbContext.Hops.Find(hopCode);
        }
    }
}