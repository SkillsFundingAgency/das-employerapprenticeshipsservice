using FluentAssertions;
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
        InvitationStatus.Accepted.GetTeamMemberStatus().Should().Be("Active");
    }
    [Test]
    public void ItShouldRenderAwaiting()
    {
        InvitationStatus.Pending.GetTeamMemberStatus().Should().Be("Invitation awaiting response");
    }
    [Test]
    public void ItShouldRenderExpired()
    {
        InvitationStatus.Expired.GetTeamMemberStatus().Should().Be("Invitation expired");
    }
    [Test]
    public void ItShouldRenderEmpty()
    {
        InvitationStatus.Deleted.GetTeamMemberStatus().Should().Be(string.Empty);
    }
}