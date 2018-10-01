using AutoMapper;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Mappings;

namespace SFA.DAS.EmployerAccounts.UnitTests.Mappings
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