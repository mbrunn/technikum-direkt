using System;
using System.Net.Http;

namespace TechnikumDirekt.IntegrationTests
{
    public class IntegrationTests : IClassFixture<TechnikumDirekt.Services.Startup>, IDisposable
    {
        protected readonly HttpClient Client;
        protected readonly CustomWebApplicationFactory<TechnikumDirekt.Services.Startup> Factory;
        
        protected IntegrationTests()
        {
            Factory = new CustomWebApplicationFactory<TechnikumDirekt.Services.Startup>();
            Client = Factory.CreateClient();
        }

        /// <summary>
        /// Dispose of Client and TestApp
        /// </summary>
        public void Dispose()
        {
            Client.Dispose();
            Factory.Dispose();
        }
    }

    public interface IClassFixture<T>
    {
    }
}