using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountTransactionOrchestratorTests
{
    public class WhenIGetAccountTransactions
    {
        private const string HashedAccountId = "123ABC";
        private const string ExternalUser = "Test user";

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
                .ReturnsAsync(new GetEmployerAccountTransactionsResponse
                {
                    Data = new AggregationData
                    {
                        TransactionLines = new List<TransactionLine>
                        {
                            new LevyDeclarationTransactionLine()
                        }
                    }
                });

            _orchestrator = new EmployerAccountTransactionsOrchestrator(_mediator.Object);
        }

        [Test]
        [TestCase(2,2017)]
        [TestCase(6, 2017)]
        [TestCase(8, 2019)]
        [TestCase(12, 2020)]
        public async Task ThenARequestShouldBeMadeForTransactions(int month, int year)
        {
            //Arrange
            var daysInMonth = DateTime.DaysInMonth(year, month);

            //Act
           await _orchestrator.GetAccountTransactions(HashedAccountId, year, month, ExternalUser);

            //Assert
            _mediator.Verify(x=> x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(
                q => q.FromDate.Equals(new DateTime(year, month, 1)) &&
                q.ToDate.Equals(new DateTime(year, month, daysInMonth)))), Times.Once);
        }

        [Test]
        public async Task ThenARequestShouldBeMadeForTransactionsForCurrentMonthIfNoYearOrMonthHasBeenGiven()
        {
            //Arrange
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var daysInMonth = DateTime.DaysInMonth(year, month);

            //Act
            await _orchestrator.GetAccountTransactions(HashedAccountId, default(int), default(int), ExternalUser);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(
                q => q.FromDate.Equals(new DateTime(year, month, 1)) &&
                     q.ToDate.Equals(new DateTime(year, month, daysInMonth)))), Times.Once);
        }
    }
}
