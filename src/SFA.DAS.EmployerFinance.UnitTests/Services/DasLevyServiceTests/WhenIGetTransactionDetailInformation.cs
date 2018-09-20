using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Queries.GetAccountLevyTransactions;
using SFA.DAS.EmployerFinance.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.DasLevyServiceTests
{
    public class WhenIGetTransactionDetailInformation
    {
        private DasLevyService _dasLevyService;
        private Mock<IMediator> _mediator;
        private Mock<ITransactionRepository> _transactionRepository;
        private DateTime _fromDate;
        private DateTime _toDate;
        private long _accountId;

        [SetUp]
        public void Arrange()
        {
            _fromDate = DateTime.Now.AddDays(-10);
            _toDate = DateTime.Now.AddDays(-2);
            _accountId = 2;

            _transactionRepository = new Mock<ITransactionRepository>();

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLevyTransactionsQuery>()))
                     .ReturnsAsync(new GetAccountLevyTransactionsResponse
                     {
                         Transactions = new List<TransactionLine>
                        {
                            new LevyDeclarationTransactionLine()
                        }
                     });

            _dasLevyService = new DasLevyService(_mediator.Object, _transactionRepository.Object);
        }
        [Test]
        public async Task ThenTheMediatorMethodIsCalled()
        {
            //Act
            await _dasLevyService.GetAccountLevyTransactionsByDateRange<LevyDeclarationTransactionLine>
                        (_accountId, _fromDate, _toDate);

            //Assert
            _mediator.Verify(x =>
                x.SendAsync(It.Is<GetAccountLevyTransactionsQuery>(c =>
                    c.AccountId.Equals(_accountId) &&
                    c.FromDate.Equals(_fromDate) &&
                    c.ToDate.Equals(_toDate))), Times.Once);
        }
        [Test]
        public async Task ThenTheResponseFromTheQueryIsReturned()
        {
            //Act
            var actual = await _dasLevyService.GetAccountLevyTransactionsByDateRange<LevyDeclarationTransactionLine>
                                    (_accountId, _fromDate, _toDate);

            //Assert
            Assert.IsNotEmpty(actual);
        }
        [Test]
        public async Task ThenIfNullIsReturnedFromTheResponseEmptyListIsReturnedFromTheCall()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLevyTransactionsQuery>()))
                .ReturnsAsync(new GetAccountLevyTransactionsResponse()
                {
                    Transactions = null
                });

            //Act
            var actual = await _dasLevyService.GetAccountLevyTransactionsByDateRange<LevyDeclarationTransactionLine>
                                    (_accountId, _fromDate, _toDate);

            //Assert
            Assert.IsEmpty(actual);
        }
    }
}
