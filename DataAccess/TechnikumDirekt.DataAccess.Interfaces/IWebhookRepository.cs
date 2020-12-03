using System.Collections;
using System.Collections.Generic;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Interfaces
{
    public interface IWebhookRepository
    {
        IEnumerable<Webhook> GetAll();

        IEnumerable<Webhook> GetAllSubscribersByTrackingId(string trackingId);

        void RemoveSubscription(long webhookId);

        void AddSubscription(Webhook webhook);
    }
}