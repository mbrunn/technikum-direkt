using TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.ServiceAgents.Interfaces
{
    public interface ILogisticsPartnerAgent
    {        
        public bool TransitionParcelToPartner(string trackingId, Parcel parcel, Transferwarehouse transferWarehouse);
    }
}