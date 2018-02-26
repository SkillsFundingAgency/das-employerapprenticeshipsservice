using AutoMapper;
using NUnit.Framework;

namespace SFA.DAS.EAS.TestCommon
{
    [SetUpFixture]
    public abstract class ArrangeTest
    {
        [OneTimeSetUp]
        public void Arrange()
        {
            Mapper.Initialize(c =>
            {
                c.AddProfile<Application.Mappings.AccountMaps>();
                c.AddProfile<Application.Mappings.MembershipMaps>();
                c.AddProfile<Application.Mappings.TransferConnectionInvitationMaps>();
                c.AddProfile<Application.Mappings.UserMaps>();
                c.AddProfile<Web.Mappings.ActivityMaps>();
                c.AddProfile<Web.Mappings.TransactionMaps>();
                c.AddProfile<Web.Mappings.TransferConnectionInvitationMaps>();
            });
        }
    }
}