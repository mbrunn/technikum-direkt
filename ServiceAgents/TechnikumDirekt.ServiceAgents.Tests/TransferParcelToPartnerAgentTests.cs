using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using TechnikumDirekt.BusinessLogic.Models;
using TechnikumDirekt.ServiceAgents.Exceptions;
using TechnikumDirekt.ServiceAgents.Models;
using TechnikumDirekt.Services.Models;
using Parcel = TechnikumDirekt.Services.Models.Parcel;
using Recipient = TechnikumDirekt.Services.Models.Recipient;
using Transferwarehouse = TechnikumDirekt.Services.Models.Transferwarehouse;

namespace TechnikumDirekt.ServiceAgents.Tests
{
    public class TransferParcelToPartnerAgentTests
    {
        private TransferParcelToPartnerAgent _parcelToPartnerAgent;

        private const string ValidTrackingId = "ABCD1234";
        
        private readonly Parcel _validParcel = new Parcel
        {
            Weight = 5.0f,
            Recipient = new Recipient(),
            Sender = new Recipient(),
        };
        
        private readonly Transferwarehouse _validTransferWarehouse = new Transferwarehouse()
        {
            LogisticsPartner = "Yeetmann Gruppe",
            LogisticsPartnerUrl = "http://yeetmann-gruppe.at/",
            Code = "HOPCO123",
            Description = "Yeeting since 1983",
            HopType = "Transferwarehouse",
            LocationCoordinates = new GeoCoordinate(),
            LocationName = "Yeetmann Gruppe HQ",
            ProcessingDelayMins = 10,
            RegionGeoJson = "geoJson FTW"
        };
        
        [SetUp]
        public void Setup()
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            
            var clientHandlerStub = new DelegatingHandlerStub((request, token) =>
            {
                var response = new HttpResponseMessage();
                
                response.StatusCode = HttpStatusCode.OK;

                if (request.RequestUri.AbsoluteUri.Contains("invalidUrl"))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Content = new StringContent("[]");
                } 
                else
                {
                    response.Content = new StringContent(@"[{""trackingId"": ""string""}]");
                }
                
                return Task.FromResult(response);
            });
            
            var client = new HttpClient(clientHandlerStub) { BaseAddress = new Uri("https://example.com") };

            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            _parcelToPartnerAgent = new TransferParcelToPartnerAgent(mockFactory.Object);
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
        
        #region TransitionParcelToPartner

        [Test]
        public void TransitionParcelToPartner_ReturnsPoint_WithValidAddress()
        {
            var success = _parcelToPartnerAgent.TransitionParcelToPartner(ValidTrackingId, _validParcel, _validTransferWarehouse);
            
            Assert.IsTrue(success);
        }
        
        [Test]
        public void TransitionParcelToPartner_ThrowArgumentNullException_WithNullAddress()
        {
            Assert.Throws<ArgumentNullException>(() => _parcelToPartnerAgent.TransitionParcelToPartner(null, null, null));
        }
        
        //TODO: Not working but WHY ?!?!?!?!?
        [Test]
        public void TransitionParcelToPartner_UnsuccessfulResponse_WithInvalidUrl()
        {
            var inValidTransferWarehouse = _validTransferWarehouse;
            inValidTransferWarehouse.LogisticsPartnerUrl = "http://invalidUrl.ru/";

            Assert.Throws<ServiceAgentsNotFoundException>(() => _parcelToPartnerAgent.TransitionParcelToPartner(ValidTrackingId, _validParcel, inValidTransferWarehouse));
        }

        #endregion
    }
}