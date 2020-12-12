using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace TechnikumDirekt.IntegrationTests
{
    public class IntegrationTests : IClassFixture<TechnikumDirekt.Services.Startup>, IDisposable
    {
        protected readonly HttpClient Client;
        protected readonly CustomWebApplicationFactory<TechnikumDirekt.Services.Startup> Factory;
        
        protected IntegrationTests()
        {
            Factory = new CustomWebApplicationFactory<TechnikumDirekt.Services.Startup>();
            
            var clientOptions = new WebApplicationFactoryClientOptions();
            clientOptions.BaseAddress = Factory.ClientOptions.BaseAddress;
            Client = Factory.CreateClient(clientOptions);
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