using NUnit.Framework;
using SFA.DAS.EAS.Support.Web.Services;

namespace SFA.DAS.EAS.Support.Web.Tests.Services;

[TestFixture]
public class WhenTestingChallengeHelper
{
    [Test]
    public void ItShouldProvideTheChallengeMessageForCharacters0And1()
    {
        var challengeCharacters = new List<int> { 0, 1 };
        var actual = ChallengeHelper.GetChallengeMessage(challengeCharacters);

        Assert.AreEqual($"1st & 2nd character of a PAYE scheme (excluding the /):", actual);
    }

    [Test]
    public void ItShouldProvideTheChallengeMessageForCharacters21And22()
    {
        var challengeCharacters = new List<int> { 21, 22 };
        var actual = ChallengeHelper.GetChallengeMessage(challengeCharacters);

        Assert.AreEqual($"22nd & 23rd character of a PAYE scheme (excluding the /):", actual);
    }

    [Test]
    public void ItShouldProvideTheChallengeMessageForCharacters2And3()
    {
        var challengeCharacters = new List<int> { 2, 3 };
        var actual = ChallengeHelper.GetChallengeMessage(challengeCharacters);

        Assert.AreEqual($"3rd & 4th character of a PAYE scheme (excluding the /):", actual);
    }
}