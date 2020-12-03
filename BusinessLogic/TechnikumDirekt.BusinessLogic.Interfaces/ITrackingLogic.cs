using System.Collections.Generic;
using TechnikumDirekt.BusinessLogic.Models;

namespace TechnikumDirekt.BusinessLogic.Interfaces
{
    public interface ITrackingLogic
    {
        /// <summary> Marks a parcel as delivered to customer. </summary>
        /// <param name="trackingId">Tracking id of the delivered parcel</param>
        public void ReportParcelDelivery(string trackingId);

        /// <summary> Reports a parcels arrival at the specified hop. </summary>
        /// <param name="trackingId">Tracking id of the parcel</param>
        /// <param name="code">Code of the warehouse the parcel arrived at</param>
        public void ReportParcelHop(string trackingId, string code);

        /// <summary> Submits a new parcel to the system. </summary>
        /// <param name="parcel">Parcel to submit</param>
        public string SubmitParcel(Parcel parcel);

        /// <summary> Returns tracking information of the specified parcel. </summary>
        /// <param name="trackingId">Tracking id of the parcel</param>
        public Parcel TrackParcel(string trackingId);

        /// <summary> Register a parcel received from a partner in the system. </summary>
        /// <param name="parcel">Parcel to register</param>
        /// <param name="trackingId">Tracking id of parcel to register</param>
        public void TransitionParcelFromPartner(Parcel parcel, string trackingId);

        
        //-------------------------- WEBHOOKS --------------------------------
        /// <summary>
        /// retrieve all subscribers by trackingId
        /// </summary>
        /// <param name="trackingId"></param>
        /// <returns></returns>
        public IEnumerable<Webhook> GetAllSubscribersByTrackingId(string trackingId);

        /// <summary>
        /// subscribe to a parcel with given url
        /// </summary>
        /// <param name="trackingId">trackingId of the parcel that should be tracked.</param>
        /// <param name="url">URl that will be informed of future changes.</param>
        /// <returns></returns>
        public Webhook SubscribeParcelWebhook(string trackingId, string url);

        /// <summary>
        /// remove a webhook with given id.
        /// </summary>
        /// <param name="id">Id of webhook that should be removed.</param>
        public void RemoveParcelWebhook(long id);
    }
}