using AutoMapper;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Mappings;

namespace SFA.DAS.EAS.Application.UnitTests
{
    [SetUpFixture]
    public class ArrangeApplicationTest
    {
        [OneTimeSetUp]
        public void Arrange()
        {
            Mapper.Initialize(c =>
            {
                c.AddProfile<AccountMaps>();
                c.AddProfile<TransferConnectionInvitationMaps>();
                c.AddProfile<UserMaps>();
            });
        }
    }
}