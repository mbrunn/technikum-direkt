using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Microsoft.Extensions.Logging;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;
using TechnikumDirekt.DataAccess.Sql.Exceptions;

namespace TechnikumDirekt.DataAccess.Sql
{
    public class WebhookRepository : IWebhookRepository
    {
        private readonly ITechnikumDirektContext _dbContext;
        private readonly ILogger<WebhookRepository> _logger;
        
        public WebhookRepository(ITechnikumDirektContext dbContext, ILogger<WebhookRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        public IEnumerable<Webhook> GetAll()
        {
            var webhooks = _dbContext.Webhooks.ToList();
            _logger.LogTrace($"Read" + webhooks.Count + " Webhooks out of the repository");
            return webhooks;
        }

        public IEnumerable<Webhook> GetAllSubscribersByTrackingId(string trackingId)
        {
            if(string.IsNullOrEmpty(trackingId)) throw new DataAccessArgumentNullException("trackingId can't be null for Webhook extraction.");

            try
            {
                var webhooks = _dbContext.Webhooks.Where(webhook => webhook.Parcel.TrackingId == trackingId).ToList();
                _logger.LogTrace($"{webhooks.Count()}Webhooks for trackingId {trackingId} have been found.");
                return webhooks;
            }
            catch (Exception e)
            {
                _logger.LogTrace($"Webhooks for trackingId {trackingId} couldn't be found.");
                throw new DataAccessNotFoundException($"Webhooks for trackingId {trackingId} couldn't be found.");
            }
        }

        public void RemoveSubscription(long webhookId)
        {
            var webhook = _dbContext.Webhooks.Find(webhookId);
            
            if (webhook == null)
            {
                _logger.LogTrace($"Webhook with Id {webhookId} does not exist.  WebhookRemoval failed.");
                throw new DataAccessNotFoundException($"Webhook with Id {webhookId} does not exist.  WebhookRemoval failed.");
            }
            
            _dbContext.Webhooks.Remove(webhook);
            _dbContext.SaveChanges();
        }

        public void AddSubscription(Webhook webhook)
        {
            _dbContext.Webhooks.Add(webhook);
            _dbContext.SaveChanges();
        }
    }
}