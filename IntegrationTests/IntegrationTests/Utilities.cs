using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Sql;
using FizzWare.NBuilder;
using TechnikumDirekt.BusinessLogic.Models;
using blWarehouse = TechnikumDirekt.BusinessLogic.Models.Warehouse;

namespace IntegrationTests
{
    public static class Utilities
    {
        #region DbContextUtils
        public static DbContextOptions<TechnikumDirektContext> TestDbContextOptions()
        {
            // Create a new service provider to create a new in-memory database.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance using an in-memory database and 
            // IServiceProvider that the context should resolve all of its 
            // services from.
            var builder = new DbContextOptionsBuilder<TechnikumDirektContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
        #endregion

        public static void InitializeDbForTests(ITechnikumDirektContext db)
        {
            var hierarchySpec = Builder<HierarchySpec<blWarehouse>>.CreateNew()
                .With(x => x.AddMethod = (parent, child) => parent.NextHops.Add(new WarehouseNextHops(){
                    Hop = child, TraveltimeMins = null}))
                .With(x => x.Depth = 10)
                .With(x => x.MaximumChildren = 3)
                .With(x => x.MinimumChildren = 2)
                .With(x => x.NumberOfRoots = 1).Build();

            //var list = Builder<blWarehouse>.CreateListOfSize(1).BuildHierarchy(hierarchySpec);

            
                
            return;
        }
    }
}
