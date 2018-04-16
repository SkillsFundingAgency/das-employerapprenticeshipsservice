using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.HashingService;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountTransactionsControllerTests
{
    public class WhenIViewFinanceDashboard
    {
        private Web.Controllers.EmployerAccountTransactionsController _controller;
        private Mock<EmployerAccountTransactionsOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IFeatureToggleService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private Mock<IHashingService> _hashingService;
        private Mock<IMediator> _mediator;

        private const decimal CurrentLevyFunds = 12345;
        private const string HashedAccountId = "Test";

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<EmployerAccountTransactionsOrchestrator>();
            _owinWrapper = new Mock<IAuthenticationService>();
            _featureToggle = new Mock<IFeatureToggleService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _hashingService = new Mock<IHashingService>();
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

            _controller = new Web.Controllers.EmployerAccountTransactionsController(_owinWrapper.Object,
                _featureToggle.Object, _hashingService.Object, _mediator.Object,
                _orchestrator.Object, _userViewTestingService.Object, _flashMessage.Object,
                Mock.Of<ITransactionFormatterFactory>(), Mock.Of<IMapper>());
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
