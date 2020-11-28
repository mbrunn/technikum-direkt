using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using TechnikumDirekt.ServiceAgents.Exceptions;
using TechnikumDirekt.ServiceAgents.Models;

namespace TechnikumDirekt.ServiceAgents.Tests
{
    public class OsmGeoEncodingAgentTests
    {
        private OsmGeoEncodingAgent _encodingAgent;
        private NullLogger<OsmGeoEncodingAgent> _logger;
        
        [SetUp]
        public void Setup()
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            var clientHandlerStub = new DelegatingHandlerStub(((request, token) =>
            {
                var response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.OK;

                if (request.RequestUri.Query.Contains("nonexistent"))
                {
                    response.Content = new StringContent("[]");
                }
                else
                {
                    response.Content = new StringContent(@"[{""lat"":""52.5487921"",""lon"":""-1.8164308339635031""}]");
                }
                return Task.FromResult(response);
            }));
            var client = new HttpClient(clientHandlerStub) { BaseAddress = new Uri("https://example.com") };

            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            
            _logger = new NullLogger<OsmGeoEncodingAgent>();

            _encodingAgent = new OsmGeoEncodingAgent(mockFactory.Object, _logger);
        }

        private class DelegatingHandlerStub : DelegatingHandler {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;
            public DelegatingHandlerStub() {
                _handlerFunc = (request, cancellationToken) => Task.FromResult(new HttpResponseMessage());
            }

            public DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc) {
                _handlerFunc = handlerFunc;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
                return _handlerFunc(request, cancellationToken);
            }
        }


        #region EncodeAddress

        [Test]
        public void EncodeAddress_ReturnsPoint_WithValidAddress()
        {
            var address = new Address
            {
                PostalCode = "1200",
                City = "Wien",
                Street = "Wienerstraße",
                Country = "Österreich"
            };

            var point = _encodingAgent.EncodeAddress(address);
            
            Assert.IsNotNull(point);
        }
        
        [Test]
        public void EncodeAddress_ThrowArgumentNullException_WithNullAddress()
        {
            Assert.Throws<ArgumentNullException>(() => _encodingAgent.EncodeAddress(null));
        }
        
        [Test]
        public void EncodeAddress_ReturnsNull_WithNonexistentAddress()
        {
            var address = new Address
            {
                PostalCode = "1200",
                City = "Wien",
                Street = "nonexistent",
                Country = "Austria"
            };
            
            Assert.Throws<ServiceAgentsNotFoundException>(() => _encodingAgent.EncodeAddress(address));
        }

        #endregion
    }
}