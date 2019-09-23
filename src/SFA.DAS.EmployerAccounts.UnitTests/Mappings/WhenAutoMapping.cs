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
            var config = new MapperConfiguration(c =>
            {
                c.AddProfiles(typeof(HealthCheckMappings));
                c.AddProfile<AccountMappings>();
                c.AddProfile<AgreementMappings>();
                c.AddProfile<EmploymentAgreementStatusMappings>();
                c.AddProfile<LegalEntityMappings>();
                c.AddProfile<MembershipMappings>();
                c.AddProfile<TransferConnectionInvitationMappings>();
                c.AddProfile<UserMappings>();
            });

            config.AssertConfigurationIsValid();
        }
    }
}