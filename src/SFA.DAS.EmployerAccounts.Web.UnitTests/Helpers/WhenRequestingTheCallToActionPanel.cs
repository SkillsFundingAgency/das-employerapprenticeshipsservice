using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Helpers
{
    [TestFixture]
    public class WhenRequestingTheCallToActionPanel
    {
        private IHomepagePanelViewHelper _sut;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _sut = new HomepagePanelViewHelper();
        }

        [Test, Category("UnitTest")]
        public void WhenAgreenentHasNotBeenSigned_ShouldAllowPromptToReserveFunding()
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                AgreementsToSign = true
            };

            // Act
            var panelViewResult = _sut.GetPanel1Action(model);

            // Assert
            panelViewResult.ViewName.Should().Be("SignAgreement");
        }

        [Test, Category("UnitTest")]
        public void WhenAgreenentHasBeenSigned_ShouldAllowPromptToReserveFunding()
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                AgreementsToSign = false
            };

            // Act
            var panelViewResult = _sut.GetPanel1Action(model);

            // Assert
            panelViewResult.ViewName.Should().Be("CheckFunding");
        }
    }
}
