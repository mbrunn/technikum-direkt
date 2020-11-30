using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class LogisticsPartnerApiTests : IntegrationTests, IDisposable
    { 
        [OneTimeSetUp]
        public void Setup()
        {
            
        }

        [Test]
        public async Task PostTransferParcelFromPartner_validParcelValidTrackingId_OkValidTrackingId()
        {
            
        }
        
        [Test]
        public async Task PostTransferParcelFromPartner_validParcelTrackingIdAlreadyTaken_BadRequest()
        {
            
        }
    }
}