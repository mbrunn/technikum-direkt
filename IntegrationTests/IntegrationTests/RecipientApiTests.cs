using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class RecipientApiTests : IntegrationTests, IDisposable
    { 
        [OneTimeSetUp]
        public void Setup()
        {
            
        }

        [Test]
        public async Task GetTrackingInformation_validTrackingId_OkTrackingInfo()
        {
            
        }
        
        [Test]
        public async Task GetTrackingInformation_invalidTrackingId_BadRequest()
        {
            
        }
    }
}