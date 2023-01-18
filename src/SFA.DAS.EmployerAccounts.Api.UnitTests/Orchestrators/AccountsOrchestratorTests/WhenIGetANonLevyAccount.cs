using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Orchestrators.AccountsOrchestratorTests
{
    internal class WhenIGetANonLevyAccount 
    {
        private AccountsOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _log;
    
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _log = new Mock<ILog>();
            _orchestrator = new AccountsOrchestrator(_mediator.Object, _log.Object, Mock.Of<IMapper>(), Mock.Of<IHashingService>());  
        }
       
        [Test]
        public async Task ThenResponseShouldHaveAccountAgreementTypeSetToNonLevy()
        {
            //Arrange
            var agreementType = AgreementType.NonLevyExpressionOfInterest;
            const string hashedAgreementId = "ABC123";

            var response = new GetEmployerAccountDetailByHashedIdResponse
            {
                Account = new AccountDetail
                {
                    AccountAgreementTypes = new List<AgreementType>
                    {
                        agreementType,
                        agreementType
                    }
                }
            };

            _mediator
                .Setup(x => x.Send(It.IsAny<GetEmployerAccountDetailByHashedIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response)
                .Verifiable("Get account was not called");

            //Act
            var result = await _orchestrator.GetAccount(hashedAgreementId);

            //Assert
            Assert.AreEqual(agreementType.ToString(), result.AccountAgreementType.ToString());
        }
    }
}
