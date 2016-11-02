using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountBalancesTests
{
    public class WhenIGetAccountBalances :QueryBaseTest<GetAccountBalancesQueryHandler,GetAccountBalancesRequest,GetAccountBalancesResponse>
    {
        private Mock<IDasLevyRepository> _repository;
        public override GetAccountBalancesRequest Query { get; set; }
        public override GetAccountBalancesQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountBalancesRequest>> RequestValidator { get; set; }
        private List<long> _expectedAccountIds;

        [SetUp]
        public void Arrange()
        {
            _expectedAccountIds = new List<long> {234234,515151,12312312};

            SetUp();

            _repository = new Mock<IDasLevyRepository>();
            _repository.Setup(x => x.GetAccountBalances(It.IsAny<List<long>>())).ReturnsAsync(new List<AccountBalance> {new AccountBalance()});

            Query = new GetAccountBalancesRequest {AccountIds = _expectedAccountIds };

            RequestHandler = new GetAccountBalancesQueryHandler(_repository.Object,RequestValidator.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x=>x.GetAccountBalances(_expectedAccountIds));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotEmpty(actual.Accounts);
        }
    }
}
