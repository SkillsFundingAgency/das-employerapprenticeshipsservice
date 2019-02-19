using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerFinance.Queries.GetExpiringAccountFunds;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.EmployerFinance.Web.ViewModels;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.EmployerAccountTransactionsControllerTests
{
    public class WhenIViewFinanceDashboard
    {
        private const string ExpectedHashedAccountId = "ABC123";
        private const long ExpectedAccountId = 12;
        private const decimal ExpectedCurrentFunds = 123.45M;
        private const decimal ExpectedExpiringFundsAmount = 20.34M;
        
        private readonly DateTime _expectedExpiringFundsExpiryDate = DateTime.Now.AddMonths(2);

        private EmployerAccountTransactionsController _controller;
        private Mock<EmployerAccountTransactionsOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IMediator> _mediator;
        private GetAccountFinanceOverviewQuery _query;
        private GetAccountFinanceOverviewResponse _response;
        
        [SetUp]
        public void Arrange()
        {
            _query = new GetAccountFinanceOverviewQuery
            {
                AccountHashedId = ExpectedHashedAccountId
            };

            _response = new GetAccountFinanceOverviewResponse
            {
                AccountId = ExpectedAccountId,
                CurrentFunds = ExpectedCurrentFunds,
                ExpiringFundsExpiryDate = _expectedExpiringFundsExpiryDate,
                ExpiringFundsAmount = ExpectedExpiringFundsAmount
            };

            _orchestrator = new Mock<EmployerAccountTransactionsOrchestrator>();
            _owinWrapper = new Mock<IAuthenticationService>();
            _mediator = new Mock<IMediator>();

            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(() => _response);

            _controller = new EmployerAccountTransactionsController(
                _owinWrapper.Object,
                _orchestrator.Object,
                Mock.Of<IMapper>(),
                _mediator.Object);
        }

        [Test]
        public async Task ThenThAccountHashedIdIsReturned()
        {
            //Act
            var result = await _controller.Index(_query);

            //Assert
            var viewResult = result as ViewResultBase;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as OrchestratorResponse<FinanceDashboardViewModel>;
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Data);
            Assert.AreEqual(ExpectedHashedAccountId, model.Data.AccountHashedId);
        }

        [Test]
        public async Task ThenTheViewModelHasTheCorrectLevyBalance()
        {
            //Act
            var result = await _controller.Index(_query);

            //Assert
            var viewResult = result as ViewResultBase;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as OrchestratorResponse<FinanceDashboardViewModel>;
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Data);
            Assert.AreEqual(ExpectedCurrentFunds, model.Data.CurrentLevyFunds);
        }
    }
}
