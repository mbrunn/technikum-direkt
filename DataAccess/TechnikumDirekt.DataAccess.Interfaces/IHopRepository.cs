using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Interfaces
{
    public interface IHopRepository
    {
        Hop GetHopByCode(string hopCode);
    }
}