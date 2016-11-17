using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactions;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.DasLevyServiceTests
{
    public class WhenIGetTransactionInformation
    {
        private DasLevyService _dasLevyService;
        private Mock<IMediator> _mediator;
        private const long ExpectedAccountId = 545645647;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTransactionsRequest>())).ReturnsAsync(new GetAccountTransactionsResponse { TransactionLines = new List<TransactionLine> { new TransactionLine() } });

            _dasLevyService = new DasLevyService(_mediator.Object);    
        }

        [Test]
        public async Task ThenTheMediatorMethodIsCalled()
        {
            //Act
            await _dasLevyService.GetTransactionsByAccountId(ExpectedAccountId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetAccountTransactionsRequest>(c=>c.AccountId.Equals(ExpectedAccountId))), Times.Once);
        }

        [Test]
        public async Task ThenTheResponseFromTheQueryIsReturned()
        {
            //Act
            var actual = await _dasLevyService.GetTransactionsByAccountId(ExpectedAccountId);

            //Assert
            Assert.IsNotEmpty(actual);
        }

        [Test]
        public async Task ThenIfNullIsReturnedFromTheResponseNullIsReturnedFromTheCall()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTransactionsRequest>())).ReturnsAsync(new GetAccountTransactionsResponse {TransactionLines = null});

            //Act
            var actual = await _dasLevyService.GetTransactionsByAccountId(ExpectedAccountId);

            //Assert
            Assert.IsNull(actual);
        }

    }
}
