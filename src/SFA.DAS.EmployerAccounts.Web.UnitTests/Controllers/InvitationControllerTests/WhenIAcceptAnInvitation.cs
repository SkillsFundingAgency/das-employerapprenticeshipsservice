﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.InvitationControllerTests
{
    public class WhenIAcceptAnInvitation : ControllerTestBase
    {
        private Mock<InvitationOrchestrator> _invitationOrchestrator;
        private InvitationController _controller;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private EmployerAccountsConfiguration _configuration;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();

            _owinWrapper = new Mock<IAuthenticationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _configuration = new EmployerAccountsConfiguration();

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

            _controller = new InvitationController(_invitationOrchestrator.Object, _owinWrapper.Object, 
                _userViewTestingService.Object, _configuration, _flashMessage.Object);

            //Act
            await _controller.Accept(invitationId, invitation);

            //Assert
            _invitationOrchestrator.Verify(x=> x.AcceptInvitation(It.Is<long>(l => l==12345L), It.IsAny<string>()), Times.Once);
        }

    }
}
