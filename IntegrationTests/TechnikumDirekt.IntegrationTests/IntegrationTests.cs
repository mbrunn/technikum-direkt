using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            if (Factory.ClientOptions.BaseAddress.ToString().Contains("localhost") || string.IsNullOrEmpty(Factory.ClientOptions.BaseAddress.ToString()))
            {
                Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
            }
            else
            {
                Client = new HttpClient {BaseAddress = clientOptions.BaseAddress};
            }
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