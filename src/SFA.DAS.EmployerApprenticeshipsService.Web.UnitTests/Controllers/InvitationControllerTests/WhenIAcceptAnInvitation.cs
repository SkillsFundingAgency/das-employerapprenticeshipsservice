using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.InvitationControllerTests
{
    public class WhenIAcceptAnInvitation : ControllerTestBase
    {
        private Mock<InvitationOrchestrator> _invitationOrchestrator;
        private InvitationController _controller;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();

            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _invitationOrchestrator = new Mock<InvitationOrchestrator>();


        }


        [Test]
        public async Task ThenTheInvitationIsAccepted()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("TEST");

            var invitationId = 12345L;
            var invitation = new UserInvitationsViewModel
            {
                Invitations = new List<InvitationView>
                {
                    new InvitationView
                    {
                        Id = invitationId,
                        AccountName = "Test Account"
                    }
                }
            };

            _invitationOrchestrator.Setup(x => x.AcceptInvitation(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.FromResult<object>(null));

            _controller = new InvitationController(_invitationOrchestrator.Object, _owinWrapper.Object, _featureToggle.Object, _userViewTestingService.Object, _configuration,_flashMessage.Object);

            //Act
            await _controller.Accept(invitationId, invitation);

            //Assert
            _invitationOrchestrator.Verify(x=> x.AcceptInvitation(It.Is<long>(l => l==12345L), It.IsAny<string>()), Times.Once);
        }

    }
}
