using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using TechnikumDirekt.BusinessLogic.Models;
using TechnikumDirekt.ServiceAgents.Exceptions;
using TechnikumDirekt.ServiceAgents.Models;

namespace TechnikumDirekt.ServiceAgents.Tests
{
    public class WebhookServiceAgentTests
    {
        private WebhookServiceAgent _webhookServiceAgent;
        private NullLogger<WebhookServiceAgent> _logger;

        private const string ValidUrl = "http://yeetmann-gruppe.at";
        private const string InValidUrl = "https://nonexistent.ru/";
        
        [SetUp]
        public void Setup()
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            var clientHandlerStub = new DelegatingHandlerStub(((request, token) =>
            {
                var response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.OK;

                if (request.RequestUri.ToString() == InValidUrl)
                {
                    response.Content = new StringContent("[]");
                    response.StatusCode = HttpStatusCode.BadRequest;
                }
                return Task.FromResult(response);
            }));
            
            var client = new HttpClient(clientHandlerStub);

            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            
            _logger = new NullLogger<WebhookServiceAgent>();

            _webhookServiceAgent = new WebhookServiceAgent(mockFactory.Object, _logger);
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
        public void NotifySubscriber_DoesNotThrow_WithValidWebhook()
        {
            //Arrange
            var webhook = new Webhook()
            {
                CreationDate = DateTime.Now,
                Id = 1,
                Parcel = new Parcel()
                {
                    VisitedHops = new List<HopArrival>(),
                    FutureHops = new List<HopArrival>(),
                    State = Parcel.StateEnum.InTransportEnum
                },
                Url = ValidUrl
            };

            //Act / Assert
            Assert.DoesNotThrow(() => _webhookServiceAgent.NotifySubscriber(webhook));
        }
        
        [Test]
        public void NotifySubscriber_ThrowsServiceAgentsBadArgumentException_WithInValidWebhook()
        {
            //Arrange
            var webhook = new Webhook()
            {
                CreationDate = DateTime.Now,
                Id = 1,
                Parcel = new Parcel()
                {
                    State = null
                },
                Url = ValidUrl
            };

            //Act / Assert
            Assert.Throws<ServiceAgentsBadArgumentException>(() => _webhookServiceAgent.NotifySubscriber(webhook));
        }
        
        [Test]
        public void NotifySubscriber_ThrowsServiceAgentsBadResponseException_WithInValidWebhookURL()
        {
            //Arrange
            var webhook = new Webhook()
            {
                CreationDate = DateTime.Now,
                Id = 1,
                Parcel = new Parcel()
                {
                    VisitedHops = new List<HopArrival>(),
                    FutureHops = new List<HopArrival>(),
                    State = Parcel.StateEnum.InTransportEnum
                },
                Url = InValidUrl
            };

            //Act / Assert
            Assert.Throws<ServiceAgentsBadResponseException>(() => _webhookServiceAgent.NotifySubscriber(webhook));
        }

        #endregion
    }
}