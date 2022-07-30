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
using SFA.DAS.EmployerFinance.Services;
using AutoMapper;
using AutoFixture;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountTransactionsControllerTests
{
    [TestFixture]
    public class WhenIGetTheTransactionSummaryForAnAccount
    {
        private AccountTransactionsController _controller;
        private Mock<IMediator> _mediator;
        private Mock<IMapper> _mapper;
        private Mock<ILog> _logger;
        private Mock<UrlHelper> _urlHelper;
        private  Mock<IEmployerFinanceApiService> _financeApiService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _urlHelper = new Mock<UrlHelper>();
            _mapper = new Mock<IMapper>();
            _financeApiService = new Mock<IEmployerFinanceApiService>();
            var orchestrator = new AccountTransactionsOrchestrator(_mediator.Object, _mapper.Object, _logger.Object, _financeApiService.Object);
            _controller = new AccountTransactionsController(orchestrator);
            _controller.Url = _urlHelper.Object;
        }

        //TODO : change the Return Type to Account.Api.Types
        [Test]
        public async Task ThenTheTransactionSummaryIsReturned()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var transactionSummaryResponse = new GetAccountTransactionSummaryResponse
            {
                Data = new List<TransactionSummary> { new TransactionSummary { Month = 1, Year = 2017 }, new TransactionSummary { Month = 2, Year = 2017 } }
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetAccountTransactionSummaryRequest>(q => q.HashedAccountId == hashedAccountId))).ReturnsAsync(transactionSummaryResponse);


            var fixture = new Fixture();
            ICollection<SFA.DAS.EAS.Finance.Api.Types.TransactionSummaryViewModel> apiResponse = new List<SFA.DAS.EAS.Finance.Api.Types.TransactionSummaryViewModel>()
            {
                 fixture.Create<SFA.DAS.EAS.Finance.Api.Types.TransactionSummaryViewModel>(),
                fixture.Create<SFA.DAS.EAS.Finance.Api.Types.TransactionSummaryViewModel>()
            };

            _financeApiService.Setup(x => x.GetTransactionSummary(hashedAccountId)).ReturnsAsync(apiResponse);

            var firstExpectedUri = "someuri";
            _urlHelper.Setup(
                    x =>
                        x.Route("GetTransactions",
                            It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, year = apiResponse.First().Year, month = apiResponse.First().Month }))))
                .Returns(firstExpectedUri);
            var secondExpectedUri = "someotheruri";
            _urlHelper.Setup(
                    x =>
                        x.Route("GetTransactions",
                            It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, year = apiResponse.Last().Year, month = apiResponse.Last().Month }))))
                .Returns(secondExpectedUri);

            //Act
            var response = await _controller.Index(hashedAccountId);

            
            //Assert            
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel>>>(response);
            var model = response as OkNegotiatedContentResult<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel>>;

            model?.Content.Should().NotBeNull();
            model?.Content.ShouldAllBeEquivalentTo(apiResponse, x => x.Excluding(y => y.Href));
            model?.Content.First().Href.Should().Be(firstExpectedUri);
            model?.Content.Last().Href.Should().Be(secondExpectedUri);
        }
    }
}
