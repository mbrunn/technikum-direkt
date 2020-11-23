using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;

namespace IntegrationTests
{
    public class DbSeedingHelper
    {
        public static void InitializeDbForTests(ITechnikumDirektContext db)
        {
            //db.Warehouses.AddRange(GetSeedingMessages());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(ITechnikumDirektContext db)
        {
            //db.Messages.RemoveRange(db.Messages);
            InitializeDbForTests(db);
        }

        public static List<Warehouse> GetSeedingMessages()
        {
           /* return new List<Warehouse>()
            {
                new Message(){ Text = "TEST RECORD: You're standing on my scarf." },
                new Message(){ Text = "TEST RECORD: Would you like a jelly baby?" },
                new Message(){ Text = "TEST RECORD: To the rational mind, " +
                                      "nothing is inexplicable; only unexplained." }
            };*/
           return null;
        }
    }
}