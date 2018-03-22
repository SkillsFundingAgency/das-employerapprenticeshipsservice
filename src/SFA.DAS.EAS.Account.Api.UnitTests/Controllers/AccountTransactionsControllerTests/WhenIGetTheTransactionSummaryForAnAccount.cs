using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionSummary;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.TestCommon.Extensions;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountTransactionsControllerTests
{
    [TestFixture]
    public class WhenIGetTheTransactionSummaryForAnAccount
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
        public async Task ThenTheTransactionSummaryIsReturned()
        {
            var hashedAccountId = "ABC123";
            var transactionSummaryResponse = new GetAccountTransactionSummaryResponse
            {
                Data = new List<TransactionSummary> { new TransactionSummary { Month = 1, Year = 2017 }, new TransactionSummary { Month = 2, Year = 2017 } }
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetAccountTransactionSummaryRequest>(q => q.HashedAccountId == hashedAccountId))).ReturnsAsync(transactionSummaryResponse);

            var firstExpectedUri = "someuri";
            _urlHelper.Setup(
                    x =>
                        x.Route("GetTransactions",
                            It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, year = transactionSummaryResponse.Data.First().Year, month = transactionSummaryResponse.Data.First().Month }))))
                .Returns(firstExpectedUri);
            var secondExpectedUri = "someotheruri";
            _urlHelper.Setup(
                    x =>
                        x.Route("GetTransactions",
                            It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, year = transactionSummaryResponse.Data.Last().Year, month = transactionSummaryResponse.Data.Last().Month }))))
                .Returns(secondExpectedUri);

            var response = await _controller.Index(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountResourceList<TransactionSummaryViewModel>>>(response);
            var model = response as OkNegotiatedContentResult<AccountResourceList<TransactionSummaryViewModel>>;

            model?.Content.Should().NotBeNull();
            model?.Content.ShouldAllBeEquivalentTo(transactionSummaryResponse.Data, x => x.Excluding(y => y.Href));
            model?.Content.First().Href.Should().Be(firstExpectedUri);
            model?.Content.Last().Href.Should().Be(secondExpectedUri);
        }
    }
}
