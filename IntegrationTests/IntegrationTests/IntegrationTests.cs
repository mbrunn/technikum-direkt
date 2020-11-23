using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Sql;
using TechnikumDirekt.Services;

namespace IntegrationTests
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
        /// Dispose of Client and TestWebApp
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