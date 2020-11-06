using Microsoft.Extensions.Logging;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Sql
{
    public class HopRepository : IHopRepository
    {
        private readonly ITechnikumDirektContext _dbContext;
        private readonly ILogger<HopRepository> _logger;

        public HopRepository(ITechnikumDirektContext dbContext, ILogger<HopRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Hop GetHopByCode(string hopCode)
        {
            var hop = _dbContext.Hops.Find(hopCode);
            if (hop != null)
            {
                _logger.LogTrace($"Hop with code {hopCode} has been found.");
            }
            else
            {
                _logger.LogTrace($"Hop with code {hopCode} couldn't be found.");
            }

            return hop;
        }
    }
}