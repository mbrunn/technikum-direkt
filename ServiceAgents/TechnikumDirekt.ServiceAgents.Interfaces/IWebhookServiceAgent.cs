using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.ServiceAgents.Interfaces
{
    public interface IWebhookServiceAgent
    {
        /// <summary>
        /// Service that notifies the URL of the webhook subscriber
        /// </summary>
        /// <param name="webhook">Webhook that should be notified</param>
        void NotifySubscriber(Webhook webhook);
    }
}