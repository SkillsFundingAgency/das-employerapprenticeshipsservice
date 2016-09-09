using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.EmployerAgreementControllerTests
{
    public class WhenIAddAnLegalEntityToAnAccount
    {
        private EmployerAgreementController _controller;
        private Mock<IEmployerAgreementOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<IEmployerAgreementOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();

            _controller = new EmployerAgreementController(_owinWrapper.Object, _orchestrator.Object);
        }
    }
}
