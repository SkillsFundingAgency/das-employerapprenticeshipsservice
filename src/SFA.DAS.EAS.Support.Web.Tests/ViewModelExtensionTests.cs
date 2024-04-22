using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Tests;

[TestFixture]
public class ViewModelExtensionTests
{
    [Test]
    public void ItShouldRenderActive()
    {
        Assert.That("Active", Is.EqualTo(InvitationStatus.Accepted.GetTeamMemberStatus()));
    }
    [Test]
    public void ItShouldRenderAwaiting()
    {
        Assert.That("Invitation awaiting response", Is.EqualTo(InvitationStatus.Pending.GetTeamMemberStatus()));
    }
    [Test]
    public void ItShouldRenderExpired()
    {
        Assert.That("Invitation expired", Is.EqualTo(InvitationStatus.Expired.GetTeamMemberStatus()));
    }
    [Test]
    public void ItShouldRenderEmpty()
    {
        Assert.That(string.Empty, Is.EqualTo(InvitationStatus.Deleted.GetTeamMemberStatus()));
    }
}