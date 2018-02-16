using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels.Transfers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransfersController
{
    public class WhenIViewTheTransfersDashboard
    {
        private const string ExpectedHashedAccountId = "Test";
        private const string ExpectedExternalUserId = "123";

        private Web.Controllers.TransfersController _controller;
        private Mock<TransferOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<TransferDashboardViewModel> _viewModel;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<TransferOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _viewModel = new Mock<TransferDashboardViewModel>();

            _owinWrapper.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(ExpectedExternalUserId);

            _orchestrator.Setup(x => x.GetTransferAllowance(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<TransferDashboardViewModel>
                {
                    Data = _viewModel.Object
                });

            _controller = new Web.Controllers.TransfersController(_owinWrapper.Object, _orchestrator.Object);
        }

        [Test]
        public async Task ThenTheResponseShouldContainAValidModel()
        {
            //Act
            var result = await _controller.Index(ExpectedHashedAccountId);

            //Assert
            var viewResult = result as ViewResultBase;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as OrchestratorResponse<TransferDashboardViewModel>;
            Assert.IsNotNull(model?.Data);
            Assert.AreEqual(_viewModel.Object, model.Data);
        }

        [Test]
        public async Task ThenTheCorrectRequestShouldBeMade()
        {
            //Act
            await _controller.Index(ExpectedHashedAccountId);

            //Assert
            _orchestrator.Verify(x => x.GetTransferAllowance(ExpectedHashedAccountId, ExpectedExternalUserId),
                Times.Once);
        }
    }
}
