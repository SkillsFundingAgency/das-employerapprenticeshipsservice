﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Api.Controllers;
using SFA.DAS.EAS.Api.Orchestrators;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.TestCommon.ObjectMothers;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountTransactionsControllerTests
{
    [TestFixture]
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
            var orchestrator = new AccountTransactionsOrchestrator(_mediator.Object, _logger.Object);
            _controller = new AccountTransactionsController(orchestrator);
            _controller.Url = _urlHelper.Object;
        }
        
        [Test]
        public async Task ThenTheTransactionsAreReturned()
        {
            var hashedAccountId = "ABC123";
            var year = 2017;
            var month = 3;
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {
                Data = new AggregationData { TransactionLines = new List<TransactionLine> { TransactionLineObjectMother.Create() } },
                AccountHasPreviousTransactions = false
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == year && q.Month == month))).ReturnsAsync(transactionsResponse);

            var response = await _controller.Index(hashedAccountId, year, month);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<TransactionsViewModel>>(response);
            var model = response as OkNegotiatedContentResult<TransactionsViewModel>;

            model?.Content.Should().NotBeNull();
            model?.Content.ShouldAllBeEquivalentTo(transactionsResponse.Data.TransactionLines);
            model?.Content.PreviousMonthUri.Should().BeNullOrEmpty();
            _urlHelper.Verify(x => x.Route(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
        }

        [Test]
        public async Task AndThereArePreviousTransactionsThenTheLinkIsCorrect()
        {
            var hashedAccountId = "ABC123";
            var year = 2017;
            var month = 1;
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {
                Data = new AggregationData { TransactionLines = new List<TransactionLine> { TransactionLineObjectMother.Create() } },
                AccountHasPreviousTransactions = true,
                Year = year,
                Month = month
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == year && q.Month == month))).ReturnsAsync(transactionsResponse);

            var expectedUri = "someuri";
            _urlHelper.Setup(x => x.Route("GetTransactions", It.Is<object>(o => o.GetHashCode() == new { hashedAccountId, year = year - 1, month = 12 }.GetHashCode()))).Returns(expectedUri);

            var response = await _controller.Index(hashedAccountId, year, month);
            var model = response as OkNegotiatedContentResult<TransactionsViewModel>;

            model?.Content.PreviousMonthUri.Should().Be(expectedUri);
        }
    }
}
