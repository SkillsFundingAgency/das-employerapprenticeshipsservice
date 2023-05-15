using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.ChallengeHandler;

[TestFixture]
public class WhenTestingChallengeHandler
{
    private Mock<IChallengeService>? _challengeService;
    
    protected IChallengeHandler? Unit;
    protected Mock<IAccountRepository>? AccountRepository;
    protected Mock<IChallengeRepository>? ChallengeRepository;
    
    [SetUp]
    public void Setup()
    {
        AccountRepository = new Mock<IAccountRepository>();
        _challengeService = new Mock<IChallengeService>();
        ChallengeRepository = new Mock<IChallengeRepository>();

        Unit = new ApplicationServices.ChallengeHandler(AccountRepository.Object, _challengeService.Object,
            ChallengeRepository.Object);
    }
}