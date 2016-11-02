using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountBalances
{
    public class WhenIGetAccountBalances
    {
        private GetAccountBalancesQueryHandler _handler;
        private Mock<IDasLevyRepository> _repository;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IDasLevyRepository>();

            _handler = new GetAccountBalancesQueryHandler(_repository.Object);
        }

        [Test]
        public async Task ThenTheDasLevyRepositoryIsCalled()
        {
            //Act
            await _handler.Handle(new GetAccountBalancesRequest());

            //Assert
            _repository.Verify(x => x.GetAccountBalances());
        }

        [Test]
        public async Task ThenTheReturnTypeIsAssignableToTheResponse()
        {
            //Act
            var actual = await _handler.Handle(new GetAccountBalancesRequest());

            //Assert
            Assert.IsAssignableFrom<GetAccountBalancesResponse>(actual);
        }

        [Test]
        public async Task ThenTheValuesReturnedFromTheRepositoryAreMappedToTheResponse()
        {
            //Arrange
            _repository.Setup(x => x.GetAccountBalances()).ReturnsAsync(new List<AccountBalance> { new AccountBalance() });

            //Act
            var actual = await _handler.Handle(new GetAccountBalancesRequest());

            //Assert
            Assert.IsNotEmpty(actual.Accounts);
        }
    }
}
