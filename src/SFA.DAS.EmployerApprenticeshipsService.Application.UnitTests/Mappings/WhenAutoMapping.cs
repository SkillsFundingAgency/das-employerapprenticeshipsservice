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
                c.AddProfile<AccountMaps>();
                c.AddProfile<MembershipMaps>();
                c.AddProfile<TransferConnectionInvitationMaps>();
                c.AddProfile<UserMaps>();
            });

            config.AssertConfigurationIsValid();
        }
    }
}