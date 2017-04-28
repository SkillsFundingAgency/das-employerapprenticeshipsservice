using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountTransactionsController
{
    public class WhenIViewTransactions
    {
        private Web.Controllers.EmployerAccountTransactionsController _controller;
        private Mock<EmployerAccountTransactionsOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<EmployerAccountTransactionsOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _orchestrator.Setup(x => x.GetAccountTransactions(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<TransactionViewResultViewModel>
                {
                    Data = new TransactionViewResultViewModel(DateTime.Now)
                    {
                        Account = new Account(),
                        Model = new TransactionViewModel
                        {
                            Data = new AggregationData()
                        },
                        AccountHasPreviousTransactions = true
                    }
                });

            _controller = new Web.Controllers.EmployerAccountTransactionsController(_owinWrapper.Object, _featureToggle.Object, _orchestrator.Object, _userViewTestingService.Object, _flashMessage.Object);
        }

        [Test]
        public async Task ThenTransactionsAreRetrievedForTheAccount()
        {
            //Act
            var result = await _controller.Index("TEST", 2017, 1);

            //Assert
            _orchestrator.Verify(x=> x.GetAccountTransactions(It.Is<string>(s => s=="TEST"), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.IsNotNull(result as ViewResult);
        }

        [Test]
        public async Task ThenPreivousTransactionsStatusIsShown()
        {
            //Act
            var result = await _controller.Index("TEST", 2017, 1);

            var viewResult = result as ViewResult;
            var viewModel = viewResult?.Model as OrchestratorResponse<TransactionViewResultViewModel>;

            //Assert
            Assert.IsNotNull(viewModel);
            Assert.IsTrue(viewModel.Data.AccountHasPreviousTransactions);
        }
    }
}
