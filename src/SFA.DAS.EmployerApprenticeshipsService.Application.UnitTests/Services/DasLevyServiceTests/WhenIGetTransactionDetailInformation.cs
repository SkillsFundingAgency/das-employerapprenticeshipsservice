﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountLevyTransactions;
using SFA.DAS.EAS.Application.Services;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.UnitTests.Services.DasLevyServiceTests
{
    public class WhenIGetTransactionDetailInformation
    {
        private DasLevyService _dasLevyService;
        private Mock<IMediator> _mediator;
        private DateTime _fromDate;
        private DateTime _toDate;
        private long _accountId;

        [SetUp]
        public void Arrange()
        {
            _fromDate = DateTime.Now.AddDays(-10);
            _toDate = DateTime.Now.AddDays(-2);
            _accountId = 2;

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLevyTransactionsQuery>()))
                     .ReturnsAsync(new GetAccountLevyTransactionsResponse
                    {
                        Transactions= new List<TransactionLine>
                        {
                            new LevyDeclarationTransactionLine()
                        }
                    });

            _dasLevyService = new DasLevyService(_mediator.Object);
        }
        
        [Test]
        public async Task ThenTheMediatorMethodIsCalled()
        {
            //Act
            await _dasLevyService.GetAccountLevyTransactionsByDateRange< LevyDeclarationTransactionLine>
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
