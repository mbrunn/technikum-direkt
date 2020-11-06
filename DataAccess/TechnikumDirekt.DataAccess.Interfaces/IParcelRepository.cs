using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Interfaces
{
    public interface IParcelRepository
    {
        Parcel GetByTrackingId(string trackingId);
        void Update(Parcel parcel);
        string Add(Parcel parcel);
        void Delete(Parcel parcel);
        void Delete(string trackingId);

        /*
         //wahrscheinlich sinnvoll:
        IEnumerable<Parcel> GetParcelsAsOldAs(DateTime dateTime);
        IEnumerable<Parcel> GetParcelsWithState(Parcel.StateEnum parcelState);       
        
        IEnumerable<Parcel> GetParcelsWithRecipient(Recipient recipient);
        IEnumerable<Parcel> GetParcelWithSender(Recipient sender);
        */
    }
}