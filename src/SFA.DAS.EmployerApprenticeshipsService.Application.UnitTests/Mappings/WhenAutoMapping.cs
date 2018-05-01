using AutoMapper;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Mappings;

namespace SFA.DAS.EAS.Application.UnitTests.Mappings
{
    [TestFixture]
    public class WhenAutoMapping
    {
        [Test]
        public void ThenShouldUseValidConfiguration()
        {
            var config = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<AgreementMappings>();
                c.AddProfile<MembershipMappings>();
                c.AddProfile<TransferConnectionInvitationMappings>();
                c.AddProfile<UserMappings>();
                c.AddProfile<EmploymentAgreementStatusMappings>();
            });

            config.AssertConfigurationIsValid();
        }
    }
}