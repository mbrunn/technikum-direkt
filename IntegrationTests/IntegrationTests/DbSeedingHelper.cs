using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;

namespace IntegrationTests
{
    public class DbSeedingHelper
    {
        public static void InitializeDbForTests(ITechnikumDirektContext dbContext)
        {
            //dbContext.Warehouses.AddRange(GetSeedingMessages());
            dbContext.SaveChanges();
        }

        public static void ReinitializeDbForTests(ITechnikumDirektContext dbContext)
        {
            DeleteAllDbEntries(dbContext);
            InitializeDbForTests(dbContext);
        }

        public static void DeleteAllDbEntries(ITechnikumDirektContext dbContext)
        {
            dbContext.Database.ExecuteSqlRaw(
                $"DELETE FROM {dbContext.Model.FindEntityType(typeof(Parcel)).GetTableName()}");
            dbContext.Database.ExecuteSqlRaw(
                $"DELETE FROM {dbContext.Model.FindEntityType(typeof(Recipient)).GetTableName()}");
            dbContext.Database.ExecuteSqlRaw(
                $"DELETE FROM {dbContext.Model.FindEntityType(typeof(Hop)).GetTableName()}");
        }
    }
}