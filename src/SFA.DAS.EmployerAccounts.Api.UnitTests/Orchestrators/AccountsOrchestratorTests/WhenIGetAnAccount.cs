using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Orchestrators.AccountsOrchestratorTests
{
    internal class WhenIGetAnAccount
    {
        private AccountsOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _log;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _log = new Mock<ILog>();

            _orchestrator = new AccountsOrchestrator(_mediator.Object, _log.Object);

            _mediator
                .Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountDetailByHashedIdQuery>()))
                .ReturnsAsync(new GetEmployerAccountDetailByHashedIdResponse())
                .Verifiable("Get account was not called");
        }

        [Test]
        public async Task TheARequestToGetAccountDetailsShouldBeMade()
        {
            //Arrange
            const string hashedAgreementId = "ABC123";

            //Act
            await _orchestrator.GetAccount(hashedAgreementId);

            //Assert
            _mediator.VerifyAll();
        }
    }
}
