using System.Collections.Generic;
using NetTopologySuite.Geometries;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Interfaces
{
    public interface IHopRepository
    {
        Hop GetHopByCode(string hopCode);

        //TODO: maybe move to a separate ITruckRepository ?
        Hop GetHopContainingPoint(Point point);
    }
}