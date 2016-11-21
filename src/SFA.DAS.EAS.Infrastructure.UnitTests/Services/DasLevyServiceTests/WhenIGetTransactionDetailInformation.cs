using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.DasLevyServiceTests
{
    public class WhenIGetTransactionDetailInformation
    {
        private DasLevyService _dasLevyService;
        private Mock<IMediator> _mediator;
        private DateTime _fromDate;
        private DateTime _toDate;
        private long _accountId;
        private string _externalUserId;

        [SetUp]
        public void Arrange()
        {
            _fromDate = DateTime.Now.AddDays(-10);
            _toDate = DateTime.Now.AddDays(-2);
            _accountId = 2;
            _externalUserId = "test";

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTransactionsByDateRangeQuery>()))
                     .ReturnsAsync(new GetAccountLevyDeclationTransactionsByDateRangeResponse
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
            await _dasLevyService.GetTransactionsByDateRange< LevyDeclarationTransactionLine>
                        (_accountId, _fromDate, _toDate, _externalUserId);

            //Assert
            _mediator.Verify(x => 
                x.SendAsync(It.Is<GetAccountTransactionsByDateRangeQuery>(c => 
                    c.AccountId.Equals(_accountId) &&
                    c.FromDate.Equals(_fromDate) && 
                    c.ToDate.Equals(_toDate) &&
                    c.ExternalUserId.Equals(_externalUserId))), Times.Once);
        }

        [Test]
        public async Task ThenTheResponseFromTheQueryIsReturned()
        {
            //Act
            var actual = await _dasLevyService.GetTransactionsByDateRange<LevyDeclarationTransactionLine>
                                    (_accountId, _fromDate, _toDate, _externalUserId);

            //Assert
            Assert.IsNotEmpty(actual);
        }

        [Test]
        public async Task ThenIfNullIsReturnedFromTheResponseEmptyListIsReturnedFromTheCall()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTransactionsByDateRangeQuery>()))
                .ReturnsAsync(new GetAccountLevyDeclationTransactionsByDateRangeResponse
                {
                    Transactions = null
                });

            //Act
            var actual = await _dasLevyService.GetTransactionsByDateRange<LevyDeclarationTransactionLine>
                                    (_accountId, _fromDate, _toDate, _externalUserId);

            //Assert
            Assert.IsEmpty(actual);
        }
    }
}
