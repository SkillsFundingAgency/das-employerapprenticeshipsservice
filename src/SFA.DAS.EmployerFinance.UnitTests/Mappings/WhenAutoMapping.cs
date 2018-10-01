using AutoMapper;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Mappings;

namespace SFA.DAS.EmployerFinance.UnitTests.Mappings
{
    [TestFixture]
    public class WhenAutoMapping
    {
        [Test]
        public void ThenShouldUseValidConfiguration()
        {
            var config = new MapperConfiguration(c => c.AddProfiles(typeof(HealthCheckMappings)));

            config.AssertConfigurationIsValid();
        }
    }
}