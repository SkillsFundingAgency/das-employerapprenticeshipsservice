using NUnit.Framework;
using NUnit.Framework.Legacy;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.Services;

[TestFixture]
public class WhenTestingChallengeService
{
    private ChallengeService? _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new ChallengeService();
    }

    [Test]
    public void ItShouldObtainAnIndexListFromTheListOfPayeSchemeDetails()
    {
        var payeSchemeModel = new List<PayeSchemeModel>
        {
            new()
            {
                AddedDate = DateTime.Today.AddMonths(-12),
                Name = "Account 123",
                DasAccountId = "123",
                Ref = "123/123456",
                ObscuredPayeRef = "1**/*****6",
                RemovedDate = null
            }
        };

        var actual = _sut?.GetPayeSchemesCharacters(payeSchemeModel);

        Assert.That(actual, Is.Not.Null);
    }
}