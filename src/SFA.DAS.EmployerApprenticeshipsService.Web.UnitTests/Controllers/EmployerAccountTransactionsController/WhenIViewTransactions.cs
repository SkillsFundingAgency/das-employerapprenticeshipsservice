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

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<EmployerAccountTransactionsOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();

            _orchestrator.Setup(x => x.GetAccountTransactions(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<TransactionViewResultViewModel>
                {
                    Data = new TransactionViewResultViewModel
                    {
                        Account = new Account(),
                        Model = new TransactionViewModel
                        {
                            Data = new AggregationData()
                        }
                    }
                });

            _controller = new Web.Controllers.EmployerAccountTransactionsController(_owinWrapper.Object, _featureToggle.Object, _orchestrator.Object, _userViewTestingService.Object);
        }

        [Test]
        public async Task ThenTransactionsAreRetrievedForTheAccount()
        {
            //Act
            var result = await _controller.Index("TEST");

            //Assert
            _orchestrator.Verify(x=> x.GetAccountTransactions(It.Is<string>(s => s=="TEST"), It.IsAny<string>()), Times.Once);
            Assert.IsNotNull(result as ViewResult);
        }
    }
}
