using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Sql;
using FizzWare.NBuilder;
using TechnikumDirekt.BusinessLogic.Models;
using blWarehouse = TechnikumDirekt.BusinessLogic.Models.Warehouse;

namespace TechnikumDirekt.IntegrationTests
{
    public static class Utilities
    {
        public static string LoadDatasetLight()
        {
            using var reader = new StreamReader("Datasets/dataset_light.json");
            return reader.ReadToEnd();
        }
    }
}
