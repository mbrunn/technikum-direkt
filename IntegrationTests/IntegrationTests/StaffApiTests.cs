using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class StaffApiTests : IntegrationTests
    { 
        [OneTimeSetUp]
        public void Setup()
        {
            
        }

        [Test]
        public async Task PostReportDelivery_validTrackingId_SetsParcelStateToDelivered()
        {
            
        }
        
        [Test]
        public async Task PostReportDelivery_invalidTrackingId_BadRequest()
        {
            
        }
        
        //TODO: maybe write tests for all possible states (hopTypes)
        [Test]
        public async Task PostReportParcelHop_validTrackingIdAndHopCode_SetsParcelStateToDelivered()
        {
            
        }
    }
}