using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.FindEmployerAccountPaymentTransactions;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountTransactionOrchestratorTests
{
    public class WhenIGetAccountTransactions
    {
        private const string HashedAccountId = "123ABC";
        private const string ExternalUser = "Test user";
        private readonly DateTime _fromDate = DateTime.Now.AddDays(-20);
        private readonly DateTime _toDate = DateTime.Now.AddDays(-20);

        private Mock<IMediator> _mediator;
        private EmployerAccountTransactionsOrchestrator _orchestrator;
        private GetEmployerAccountResponse _response;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();

            _response = new GetEmployerAccountResponse
            {
                Account = new Account
                {
                    HashedId = HashedAccountId,
                    Name = "Test Account"
                }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountHashedQuery>()))
                .ReturnsAsync(_response);


            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountTransactionsQuery>()))
                .ReturnsAsync(new GetEmployerAccountTransactionsResponse());

            _orchestrator = new EmployerAccountTransactionsOrchestrator(_mediator.Object);
        }

        [Test]
        public async Task ThenARequestShouldBeMadeForTransactions()
        {
           var result = await _orchestrator.GetAccountTransactions(HashedAccountId, ExternalUser);


        }
    }
}
