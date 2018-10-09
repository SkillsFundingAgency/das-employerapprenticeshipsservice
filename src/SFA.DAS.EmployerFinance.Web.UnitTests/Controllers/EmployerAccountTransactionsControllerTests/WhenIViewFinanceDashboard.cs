using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.EmployerFinance.Web.ViewModels;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.EmployerAccountTransactionsControllerTests
{
    public class WhenIViewFinanceDashboard
    {
        private EmployerAccountTransactionsController _controller;
        private Mock<EmployerAccountTransactionsOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IMediator> _mediator;

        private const decimal CurrentLevyFunds = 12345;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<EmployerAccountTransactionsOrchestrator>();
            _owinWrapper = new Mock<IAuthenticationService>();
            _mediator = new Mock<IMediator>();

            _orchestrator.Setup(x => x.GetFinanceDashboardViewModel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<FinanceDashboardViewModel>
                {
                    Data = new FinanceDashboardViewModel
                    {
                        Account = new Account(),
                        CurrentLevyFunds = CurrentLevyFunds
                    }
                });

            _controller = new EmployerAccountTransactionsController(
                _owinWrapper.Object,
                _orchestrator.Object,
                Mock.Of<IMapper>(),
                _mediator.Object);
        }

        [Test]
        public async Task ThenTransactionsAreRetrievedForTheAccount()
        {
            //Act
            var result = await _controller.Index("HashedAccountId");

            //Assert
            _orchestrator.Verify(
                x => x.GetFinanceDashboardViewModel(It.Is<string>(s => s == "HashedAccountId"), It.Is<int>(m => m == 0),
                    It.Is<int>(m => m == 0), It.IsAny<string>()), Times.Once);

            Assert.IsNotNull(result as ViewResultBase);
        }

        [Test]
        public async Task ThenTheViewModelHasTheCorrectLevyBalance()
        {
            //Act
            var result = await _controller.Index("HashedAccountId");

            //Assert
            var viewResult = result as ViewResultBase;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as OrchestratorResponse<FinanceDashboardViewModel>;
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Data);
            Assert.AreEqual(CurrentLevyFunds, model.Data.CurrentLevyFunds);
        }
    }
}
