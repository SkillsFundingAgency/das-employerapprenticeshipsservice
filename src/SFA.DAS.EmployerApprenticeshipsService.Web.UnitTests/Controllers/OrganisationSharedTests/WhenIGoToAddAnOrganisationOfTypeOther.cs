using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.OrganisationSharedTests
{
    public class WhenIGoToAddAnOrganisationOfTypeOther : OrganisationSharedControllerTestBase
    {
        public override void SetupOrchestrator()
        {
            base.Orchestrator.Setup(x => x.GetAddOtherOrganisationViewModel(It.IsAny<string>()))
                .Returns(new OrchestratorResponse<OrganisationDetailsViewModel>());

        }

        [Test]
        public void ThenIGetTheAddOtherOrganisationView()
        {
            //Act
            var result = base.Controller.AddCustomOrganisationDetails("ABC123") as ViewResult;

            //Assert
            base.Orchestrator.Verify(x => x.GetAddOtherOrganisationViewModel(It.Is<string>(s => s == "ABC123")), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(ControllerConstants.AddOtherOrganisationDetailsViewName, result.ViewName);
        }
    }
}
