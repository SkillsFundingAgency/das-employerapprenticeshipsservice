using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountStats;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountStatsTests
{
    public class WhenIGetAccountStats : QueryBaseTest<GetAccountStatsHandler, GetAccountStatsQuery, GetAccountStatsResponse>
    {
       
        private Mock<IAccountRepository> _repository;
        private AccountStats _accountStats;
        private Mock<IHashingService> _hashingService;
        private const string HashedAccountId = "123ABC";
        private const string ExternalUserId = "456WER";
        public override GetAccountStatsQuery Query { get; set; }
        public override GetAccountStatsHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountStatsQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _accountStats = new AccountStats
            {
                AccountId = 10,
                OrganisationCount = 4,
                PayeSchemeCount = 3,
                TeamMemberCount = 8
            };

            _repository = new Mock<IAccountRepository>();

            _repository.Setup(x => x.GetAccountStats(It.IsAny<long>()))
                .ReturnsAsync(_accountStats);

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(_accountStats.AccountId);

            Query = new GetAccountStatsQuery { HashedAccountId = HashedAccountId, ExternalUserId = ExternalUserId };

            RequestHandler = new GetAccountStatsHandler(_repository.Object, _hashingService.Object, RequestValidator.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _hashingService.Verify(x => x.DecodeValue(HashedAccountId), Times.Once);
            _repository.Verify(x => x.GetAccountStats(_accountStats.AccountId), Times.Once);
        }

        public override async  Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(_accountStats.AccountId, result.Stats.AccountId);
            Assert.AreEqual(_accountStats.OrganisationCount, result.Stats.OrganisationCount);
            Assert.AreEqual(_accountStats.TeamMemberCount, result.Stats.TeamMemberCount);
            Assert.AreEqual(_accountStats.PayeSchemeCount, result.Stats.PayeSchemeCount);
        }

        [Test]
        public async Task ThenIShouldReturnNullIfAccountCannotBeFound()
        {
            //Arrange
            _repository.Setup(x => x.GetAccountStats(It.IsAny<long>()))
                .ReturnsAsync(() => null);

            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNull(result.Stats);
        }
    }
}
