using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Finance.Api.Types;
using SFA.DAS.EAS.TestCommon.Extensions;
using SFA.DAS.EmployerFinance.Api.Controllers;
using SFA.DAS.EmployerFinance.Api.Orchestrators;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Queries.GetEmployerAccountTransactions;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using TransactionItemType = SFA.DAS.EmployerFinance.Models.Transaction.TransactionItemType;

namespace SFA.DAS.EmployerFinance.Api.UnitTests.Controllers.AccountTransactionsControllerTests
{
    public class WhenIGetTransactionsForAnAccount
    {
        private AccountTransactionsController _controller;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private Mock<UrlHelper> _urlHelper;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _urlHelper = new Mock<UrlHelper>();
            _urlHelper.Setup(x => x.Route(It.IsAny<string>(), It.IsAny<object>())).Returns("dummyurl");            
            var orchestrator = new AccountTransactionsOrchestrator(_mediator.Object, _logger.Object);
            _controller = new AccountTransactionsController(orchestrator);
            _controller.Url = _urlHelper.Object;
        }

        [Test]
        public async Task ThenTheTransactionsAreReturned()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var year = 2017;
            var month = 3;
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {                
                Data = TransactionLineObjectMother.Create(),
                AccountHasPreviousTransactions = false
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == year && q.Month == month))).ReturnsAsync(transactionsResponse);
           
            //Act
            var response = await _controller.GetTransactions(hashedAccountId, year, month);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel>>(response);
            var model = response as OkNegotiatedContentResult<SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel>;
            model?.Content.Should().NotBeNull();
            model?.Content.ShouldAllBeEquivalentTo(transactionsResponse.Data.TransactionLines, options => options.Excluding(x => x.ResourceUri));
        }

        [Test]
        public async Task AndThereAreNoPreviousTransactionThenTheUrlIsNotSet()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var year = 2017;
            var month = 3;           
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {
                Data = TransactionLineObjectMother.Create(),
                AccountHasPreviousTransactions = false
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == year && q.Month == month))).ReturnsAsync(transactionsResponse);

            //Act
            var response = await _controller.GetTransactions(hashedAccountId, year, month);
            
            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<TransactionsViewModel>>(response);
            var model = response as OkNegotiatedContentResult<TransactionsViewModel>;

            model?.Content.Should().NotBeNull();
            model?.Content.PreviousMonthUri.Should().BeNullOrEmpty();
            _urlHelper.Verify(x => x.Route("GetTransactions", It.IsAny<object>()), Times.Never);
        }

        [Test]
        public async Task AndThereArePreviousTransactionsThenTheLinkIsCorrect()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var year = 2017;
            var month = 1;            
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {
                Data = TransactionLineObjectMother.Create(),
                AccountHasPreviousTransactions = true,
                Year = year,
                Month = month
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == year && q.Month == month))).ReturnsAsync(transactionsResponse);

            //Act
            var expectedUri = "someuri";
            _urlHelper.Setup(x => x.Route("GetTransactions", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, year = year - 1, month = 12 })))).Returns(expectedUri);

            //Assert
            var response = await _controller.GetTransactions(hashedAccountId, year, month);
            var model = response as OkNegotiatedContentResult<TransactionsViewModel>;

            model?.Content.PreviousMonthUri.Should().Be(expectedUri);
        }

        [Test]
        public async Task AndNoMonthIsProvidedThenTheCurrentMonthIsUsed()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var year = 2017;           
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {
                Data = TransactionLineObjectMother.Create(),
                AccountHasPreviousTransactions = false
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == year && q.Month == DateTime.Now.Month))).ReturnsAsync(transactionsResponse);

            //Act
            var response = await _controller.GetTransactions(hashedAccountId, year);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<TransactionsViewModel>>(response);
            var model = response as OkNegotiatedContentResult<TransactionsViewModel>;

            model?.Content.Should().NotBeNull();
            model?.Content.ShouldAllBeEquivalentTo(transactionsResponse.Data.TransactionLines, options => options.Excluding(x => x.ResourceUri));
            model?.Content.PreviousMonthUri.Should().BeNullOrEmpty();
            _urlHelper.Verify(x => x.Route("GetTransactions", It.IsAny<object>()), Times.Never);
        }


        [Test]
        public async Task AndThereAreLevyTransactionsThenTheLinkIsCorrect()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var year = 2017;
            var month = 1;
            var levyTransaction = TransactionLineObjectMother.Create();
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {
                Data = TransactionLineObjectMother.Create(),
                AccountHasPreviousTransactions = false,
                Year = year,
                Month = month
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == year && q.Month == month))).ReturnsAsync(transactionsResponse);

            var expectedUri = "someuri";
            _urlHelper.Setup(
                    x =>
                        x.Route("GetLevyForPeriod",
                            It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, payrollYear = levyTransaction.TransactionLines[0].PayrollYear, payrollMonth = levyTransaction.TransactionLines[0].PayrollMonth }))))
                .Returns(expectedUri);

            //Act
            var response = await _controller.GetTransactions(hashedAccountId, year, month);
            var model = response as OkNegotiatedContentResult<TransactionsViewModel>;

            //Assert            
            model?.Content[0].ResourceUri.Should().Be(expectedUri);
        }


        [Test]
        public async Task AndNoYearIsProvidedThenTheCurrentYearIsUsed()
        {
            //Arrange
            var hashedAccountId = "ABC123";           
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {
                Data = TransactionLineObjectMother.Create(),
                AccountHasPreviousTransactions = false
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == DateTime.Now.Year && q.Month == DateTime.Now.Month))).ReturnsAsync(transactionsResponse);

            //Act
            var response = await _controller.GetTransactions(hashedAccountId);

            //Assert            
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<TransactionsViewModel>>(response);
            var model = response as OkNegotiatedContentResult<TransactionsViewModel>;

            model?.Content.Should().NotBeNull();
            model?.Content.ShouldAllBeEquivalentTo(transactionsResponse.Data.TransactionLines, options => options.Excluding(x => x.ResourceUri));
        }
    }

    public static class TransactionLineObjectMother
    {
        public static AggregationData Create()
        {           
            return new AggregationData
            {
                TransactionLines =  (new TransactionLine[]
                {
                    new TransactionLine
                    {
                        AccountId = 123,
                        Balance = 500.43m,
                        Description = "Line",
                        PayrollYear = "17-18",
                        PayrollMonth = 3,
                        Amount = 100.11m,
                        DateCreated = DateTime.Now.AddDays(-1),
                        PayrollDate = DateTime.Now.AddDays(-4),
                        TransactionDate = DateTime.Now,
                        TransactionType = TransactionItemType.Declaration,
                        SubTransactions = new List<TransactionLine>()
                }   })
            };
        }        
    }
}
