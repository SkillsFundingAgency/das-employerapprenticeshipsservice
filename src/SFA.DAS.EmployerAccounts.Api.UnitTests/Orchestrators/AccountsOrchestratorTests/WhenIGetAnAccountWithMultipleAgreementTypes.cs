using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.Encoding;
using AccountDetail = SFA.DAS.EmployerAccounts.Models.Account.AccountDetail;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Orchestrators.AccountsOrchestratorTests
{
    internal class WhenIGetAnAccountWithMultipleAgreementTypes
    {
        private AccountsOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger<AccountsOrchestrator>> _log;
      
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
  
            _log = new Mock<ILogger<AccountsOrchestrator>>();
          
            _orchestrator = new AccountsOrchestrator(_mediator.Object, _log.Object, Mock.Of<IMapper>(), Mock.Of<IEncodingService>());

            var response = new GetEmployerAccountDetailByHashedIdResponse
            {
                Account = new AccountDetail
                {
                    AccountAgreementTypes = new List<AgreementType>()
                    {
                        AgreementType.Levy,
                        AgreementType.Combined
                    }
                }
            };

            _mediator
                .Setup(x => x.Send(It.IsAny<GetEmployerAccountDetailByHashedIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response)
                .Verifiable("Get account was not called");
        }

        [Test]
        public async Task ThenResponseShouldHaveAccountAgreementTypeSetToTheLatestAgreementType()
        {
            //Arrange
            AccountAgreementType agreementType = AccountAgreementType.Combined;
            const string hashedAgreementId = "ABC123";

            //Act
            var result = await _orchestrator.GetAccount(hashedAgreementId);

            //Assert
            Assert.AreEqual(agreementType, result.AccountAgreementType);
        }        
    }
}
