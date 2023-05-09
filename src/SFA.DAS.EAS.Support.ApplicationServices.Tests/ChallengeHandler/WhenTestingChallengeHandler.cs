using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.ChallengeHandler
{
    [TestFixture]
    public class WhenTestingChallengeHandler
    {
        [SetUp]
        public void Setup()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _challengeService = new Mock<IChallengeService>();
            _challengeRepository = new Mock<IChallengeRepository>();

            _unit = new ApplicationServices.ChallengeHandler(_accountRepository.Object, _challengeService.Object,
                _challengeRepository.Object);
        }

        protected IChallengeHandler _unit;
        protected Mock<IAccountRepository> _accountRepository;
        protected Mock<IChallengeService> _challengeService;
        protected Mock<IChallengeRepository> _challengeRepository;
    }
}