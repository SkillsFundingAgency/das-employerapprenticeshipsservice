using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.EmployerAccountPayeControllerTests
{
    class WhenISelectACompany
    {
        private Mock<Web.Orchestrators.EmployerAccountPayeOrchestrator> _employerAccountPayeOrchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private EmployerAccountPayeController _controller;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _userWhiteList;

        [SetUp]
        public void Arrange()
        {
            _employerAccountPayeOrchestrator = new Mock<Web.Orchestrators.EmployerAccountPayeOrchestrator>();
            _employerAccountPayeOrchestrator.Setup(x => x.RemoveSchemeFromAccount(It.IsAny<RemovePayeScheme>())).ReturnsAsync(new OrchestratorResponse<RemovePayeScheme>());
            _owinWrapper = new Mock<IOwinWrapper>();
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("123abc");
            _featureToggle = new Mock<IFeatureToggle>();
            _userWhiteList = new Mock<IUserWhiteList>();

            _controller = new EmployerAccountPayeController(
                _owinWrapper.Object, _employerAccountPayeOrchestrator.Object, _featureToggle.Object, _userWhiteList.Object);
        }

        [Test]
        public async Task ThenIfTheCompanyCannotBeFoundAFormErrorIsShown()
        {
            //Assign
            _employerAccountPayeOrchestrator.Setup(x => x.GetCompanyDetails(It.IsAny<SelectEmployerModel>()))
                .ReturnsAsync(new OrchestratorResponse<SelectEmployerViewModel>()
                {
                    Status = HttpStatusCode.BadRequest
                });

            //Act
            var result = await _controller.SelectCompany("32654632", new ConfirmNewPayeScheme
            {
                LegalEntityCode = "47365734"
            }) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.TempData.ContainsKey("companyNumberError"));

        }
    }
}
