using Parcel = TechnikumDirekt.BusinessLogic.Models.Parcel;

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
        public void SubmitParcel(Parcel parcel);

        /// <summary> Returns tracking information of the specified parcel. </summary>
        /// <param name="trackingId">Tracking id of the parcel</param>
        public Parcel TrackParcel(string trackingId);

        /// <summary> Register a parcel received from a partner in the system. </summary>
        /// <param name="parcel">Parcel to register</param>
        /// <param name="trackingId">Tracking id of parcel to register</param>
        public void TransitionParcelFromPartner(Parcel parcel, string trackingId);
    }
}