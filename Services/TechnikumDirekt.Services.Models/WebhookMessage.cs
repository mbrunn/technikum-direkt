using System.Runtime.Serialization;

namespace TechnikumDirekt.Services.Models
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class WebhookMessage : TrackingInformation
    { 
        /// <summary>
        /// Gets or Sets TrackingId
        /// </summary>
        [DataMember(Name="trackingId")]
        public string TrackingId { get; set; }
    }
}
