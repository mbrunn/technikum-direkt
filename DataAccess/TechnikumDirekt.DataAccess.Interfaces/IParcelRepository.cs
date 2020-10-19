using System;
using System.Collections;
using System.Collections.Generic;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Interfaces
{
    public interface IParcelRepository : ISearchableRepository<Parcel>
    {
        Parcel GetByTrackingId(string trackingId);
        
        
        /*
         //wahrscheinlich sinnvoll:
        IEnumerable<Parcel> GetParcelsAsOldAs(DateTime dateTime);
        IEnumerable<Parcel> GetParcelsWithState(Parcel.StateEnum parcelState);
        
        // wahrscheinlich unnötig:
        IEnumerable<Parcel> GetParcelsWithRecipient(Recipient recipient);
        IEnumerable<Parcel> GetParcelWithSender(Recipient sender);
        */
    }
}