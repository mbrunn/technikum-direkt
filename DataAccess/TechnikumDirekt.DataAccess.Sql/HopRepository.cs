using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Sql
{
    public class HopRepository : IHopRepository
    {
        private readonly ITechnikumDirektContext _dbContext;
        
        public HopRepository(ITechnikumDirektContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public Hop GetHopByCode(string hopCode)
        {
            return _dbContext.Hops.Find(hopCode);
        }
    }
}