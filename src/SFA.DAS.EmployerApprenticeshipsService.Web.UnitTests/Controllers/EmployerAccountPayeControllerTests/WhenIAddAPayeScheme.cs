using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountPayeControllerTests
{
    public class WhenIAddAPayeScheme
    {
        private Mock<Web.Orchestrators.EmployerAccountPayeOrchestrator> _employerAccountPayeOrchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private EmployerAccountPayeController _controller;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private const string ExpectedAccountId = "AFD123";
        private const string ExpectedUserId = "456TGF3";

        [SetUp]
        public void Arrange()
        {
            _employerAccountPayeOrchestrator = new Mock<Web.Orchestrators.EmployerAccountPayeOrchestrator>();
            
            _owinWrapper = new Mock<IOwinWrapper>();
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);
            _featureToggle = new Mock<IFeatureToggle>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();

            _controller = new EmployerAccountPayeController(
                _owinWrapper.Object, _employerAccountPayeOrchestrator.Object, _featureToggle.Object, _userViewTestingService.Object);
        }

        [Test]
        public async Task ThenTheAddPayeSchemeToAccountIsCalledWithTheCorrectParameters()
        {
            //Arrange
            var expectedAddNewPayeScheme = new AddNewPayeSchemeViewModel {AccessToken = "123DFG",HashedAccountId = ExpectedAccountId,PayeName = "123/ABC",RefreshToken = "987TGH"};

            _employerAccountPayeOrchestrator.Setup(
                x => x.AddPayeSchemeToAccount(It.IsAny<AddNewPayeSchemeViewModel>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<AddNewPayeSchemeViewModel>
                {
                    Status = HttpStatusCode.OK
                });
                
            //Act
            await _controller.ConfirmPayeScheme(ExpectedAccountId, expectedAddNewPayeScheme);

            //Assert
            _employerAccountPayeOrchestrator.Verify(x=>x.AddPayeSchemeToAccount(It.Is<AddNewPayeSchemeViewModel>(
                c=>c.AccessToken.Equals(expectedAddNewPayeScheme.AccessToken) &&
                c.HashedAccountId.Equals(expectedAddNewPayeScheme.HashedAccountId) &&
                c.PayeName.Equals(expectedAddNewPayeScheme.PayeName) &&
                c.RefreshToken.Equals(expectedAddNewPayeScheme.RefreshToken)
                ),ExpectedUserId));
        }

        [Test]
        public async Task ThenTheSuccessMessageIsCorrectlyPopulated()
        {
            //Arrange
            _employerAccountPayeOrchestrator.Setup(
                x => x.AddPayeSchemeToAccount(It.IsAny<AddNewPayeSchemeViewModel>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<AddNewPayeSchemeViewModel>
                {
                    Status = HttpStatusCode.OK
                });

            //Act
            await _controller.ConfirmPayeScheme(ExpectedAccountId, new AddNewPayeSchemeViewModel());

            //Assert
            Assert.IsTrue(_controller.TempData.ContainsKey("successHeader"));
        }
    }
}
