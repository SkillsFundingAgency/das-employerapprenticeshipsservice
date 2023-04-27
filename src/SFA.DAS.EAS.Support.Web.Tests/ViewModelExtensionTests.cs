using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Tests
{
    [TestFixture]
    public class ViewModelExtensionTests
    {
        [Test]
        public void ItShouldRenderActive()
        {
            Assert.AreEqual("Active", InvitationStatus.Accepted.GetTeamMemberStatus());
        }
        [Test]
        public void ItShouldRenderAwaiting()
        {
            Assert.AreEqual("Invitation awaiting response", InvitationStatus.Pending.GetTeamMemberStatus());
        }
        [Test]
        public void ItShouldRenderExpired()
        {
            Assert.AreEqual("Invitation expired", InvitationStatus.Expired.GetTeamMemberStatus());
        }
        [Test]
        public void ItShouldRenderEmpty()
        {
            Assert.AreEqual(string.Empty, InvitationStatus.Deleted.GetTeamMemberStatus());
        }
    }
}
