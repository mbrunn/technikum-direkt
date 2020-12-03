using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;
using TechnikumDirekt.DataAccess.Sql;
using TechnikumDirekt.DataAccess.Sql.Exceptions;

namespace TechnikumDirekt.DataAccess.Tests
{
    public class WebhookRepositoryTests
    {
        private ITechnikumDirektContext _technikumDirektContext;
        private IWebhookRepository _webhookRepository;
        private List<Webhook> _entities;
        private NullLogger<WebhookRepository> _logger;

        private const string ValidTrackingId = "A123BCD23";
        private const string ValidTrackingId2 = "B123BCD56";
        private const string InValidTrackingId = "A123BaD23";
        private const string NotfoundTrackingId = "000000000";
        
        private const long ExistingWebhookId = 1;
        private const long ExistingWebhookId2 = 2;
        private const long NonExistingWebhookId = 99;

        private Webhook validWebhook1 = new Webhook()
        {
            Id = ExistingWebhookId,
            Parcel = new Parcel()
            {
                TrackingId = ValidTrackingId
            }
        };
        
        private Webhook validWebhook2 = new Webhook()
        {
            Id = ExistingWebhookId2,
            Parcel = new Parcel()
            {
                TrackingId = ValidTrackingId2
            }
        };
        
        
        [SetUp]
        public void Setup()
        {
            _entities = new List<Webhook>()
            {
                validWebhook1,
                validWebhook2
            };

            var dbMock = new Mock<ITechnikumDirektContext>();
            dbMock.Setup(p => p.Webhooks).Returns(DbContextMock.GetQueryableMockDbSet<Webhook>(_entities));

            dbMock.Setup(p => p.Webhooks.Add(It.IsAny<Webhook>()));
            dbMock.Setup(p => p.Webhooks.Remove(It.IsAny<Webhook>()));
            dbMock.Setup(p => p.SaveChanges()).Returns(1);

            dbMock.Setup(p => p.Webhooks.Find(It.IsAny<object[]>()))
                .Returns<object[]>((keyValues) =>
                    (Webhook) _entities.FirstOrDefault(y => y.Id == (long) keyValues.GetValue(0)));
            
            _technikumDirektContext = dbMock.Object;
            _logger = NullLogger<WebhookRepository>.Instance;
        }

        #region GetAll
        
        [Test]
        public void GetAll_ReturnsWarehouseStructure_ValidWarehouseStructure()
        {
            _webhookRepository = new WebhookRepository(_technikumDirektContext, _logger);
            var wh = _webhookRepository.GetAll();
            Assert.NotNull(wh);
            Assert.IsInstanceOf<Webhook>(wh.FirstOrDefault());
            Assert.AreSame(validWebhook1, wh.FirstOrDefault());
        }

        #endregion
        
        #region RemoveSubscription

        [Test]
        public void RemoveSubscription_DoesNotThrow_ValidWebhookId()
        {
            _webhookRepository = new WebhookRepository(_technikumDirektContext, _logger);
            Assert.DoesNotThrow(() => _webhookRepository.RemoveSubscription(ExistingWebhookId));
        }

        [Test]
        public void RemoveSubscription_ThrowsDataAccessNotFoundException_InValidHopCode()
        {
            _webhookRepository = new WebhookRepository(_technikumDirektContext, _logger);
            Assert.Throws<DataAccessNotFoundException>(() => _webhookRepository.RemoveSubscription(NonExistingWebhookId));
        }

        #endregion
        
        #region ImportWarehouse

        [Test]
        public void AddSubscription_DoesNotThrow_InValidHopCode()
        {
            _webhookRepository = new WebhookRepository(_technikumDirektContext, _logger);
            Assert.DoesNotThrow(() => _webhookRepository.AddSubscription(validWebhook1));
        }

        #endregion
    }
}