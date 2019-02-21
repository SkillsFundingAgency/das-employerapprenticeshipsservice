using AutoMapper;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Web.Mappings;
using AgreementMappings = SFA.DAS.EmployerAccounts.Web.Mappings.AgreementMappings;
using HealthCheckMappings = SFA.DAS.EmployerAccounts.Web.Mappings.HealthCheckMappings;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Mappings
{
    [TestFixture]
    public class WhenAutoMapping
    {
        [Test]
        public void ThenShouldUseValidConfiguration()
        {
            var config = new MapperConfiguration(c =>
            {
                c.AddProfiles(typeof(HealthCheckMappings));
                c.AddProfile<ActivityMappings>();
                c.AddProfile<TransferMappings>();
                c.AddProfile<OrganisationMappings>();
                c.AddProfile<AgreementMappings>();
            });

            config.AssertConfigurationIsValid();
        }
    }
}