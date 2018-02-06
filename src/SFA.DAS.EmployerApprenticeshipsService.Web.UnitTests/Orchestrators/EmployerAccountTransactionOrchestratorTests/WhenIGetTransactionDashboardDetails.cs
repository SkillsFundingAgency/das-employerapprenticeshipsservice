using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetTransferBalance;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Orchestrators;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountTransactionOrchestratorTests
{
    class WhenIGetTransactionDashboardDetails
    {
        private const string HashedAccountId = "123ABC";
        private const string ExternalUser = "Test user";
        private const decimal ExpectedTransferBalance = 213.56M;

        private Mock<IMediator> _mediator;
        private EmployerAccountTransactionsOrchestrator _orchestrator;
        private GetTransferBalanceResponse _response;
        private Mock<ICurrentDateTime> _currentTime;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _currentTime = new Mock<ICurrentDateTime>();

            var accountResponse = new GetEmployerAccountResponse
            {
                Account = new Account
                {
                    HashedId = HashedAccountId,
                    Name = "Test Account"
                }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountHashedQuery>()))
                .ReturnsAsync(accountResponse);

            _response = new GetTransferBalanceResponse { Balance = ExpectedTransferBalance };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetTransferBalanaceRequest>()))
                     .ReturnsAsync(_response);


            _orchestrator = new EmployerAccountTransactionsOrchestrator(_mediator.Object, _currentTime.Object);
        }

        [Test]
        public async Task ThenIShouldGetCurrentTransferBalance()
        {
            //Act
            var viewModel = await _orchestrator.GetFinanceDashboardViewModel(HashedAccountId, 0, 0, ExternalUser);

            //Assert
            Assert.IsNotNull(viewModel?.Data);
            Assert.AreEqual(ExpectedTransferBalance, viewModel.Data.CurrentTransferFunds);
        }

        [Test]
        public async Task ThenIShouldAZeroBalanceIfTheTransferBalanaceCannotBeRetrieved()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetTransferBalanaceRequest>()))
                .ReturnsAsync(null);

            //Act
            var viewModel = await _orchestrator.GetFinanceDashboardViewModel(HashedAccountId, 0, 0, ExternalUser);

            //Assert
            Assert.IsNotNull(viewModel?.Data);
            Assert.AreEqual(0, viewModel.Data.CurrentTransferFunds);
        }
    }
}
