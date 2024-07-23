using NUnit.Framework;
using SFA.DAS.EAS.Support.Core.Services;

namespace SFA.DAS.EAS.Support.Core.Tests;

[TestFixture]
public class WhenTestingPayeSchemeObfuscator
{
    [TestCase("1234/56789", "1***/****9")]
    [TestCase("1AB/CD9", "1**/**9")]
    [TestCase("AB1/9CD", "A**/**D")]
    [TestCase("1/9", "1/9")]
    [TestCase("A/B", "A/B")]
    [TestCase("A/b", "A/b")]
    public void ItShouldObfuscateThePayeIdentifierDetails(string payeSchemeId, string expectedPayeSchemeId)
    {
        Assert.That(new PayeSchemeObfuscator().ObscurePayeScheme(payeSchemeId), Is.EqualTo(expectedPayeSchemeId));
    }
}