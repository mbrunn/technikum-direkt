using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class SenderApiTests : IntegrationTests, IDisposable
    { 
        [OneTimeSetUp]
        public void Setup()
        {
            
        }

        [Test]
        public async Task PostSubmitParcel_validParcel_OkValidTrackingId()
        {
            
        }
        
        [Test]
        public async Task PostSubmitParcel_invalidParcel_BadRequest()
        {
            
        }
    }
}