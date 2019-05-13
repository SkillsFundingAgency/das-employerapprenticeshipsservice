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
        private AccountDashboardViewModel _accountDashbaoardModel;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _sut = new HomepagePanelViewHelper();
        }

        [SetUp]
        public void SetUp()
        {
            _accountDashbaoardModel = new AccountDashboardViewModel
            {
                PayeSchemeCount = 1,
                AgreementsToSign = true
            };
        }

        [Test, Category("UnitTest")]
        public void WhenAgreenentHasNotBeenSigned_ShouldPromptToReserveFunding()
        {
            // Arrange

            // Act
            var panelActionResult = _sut.GetPanel1Action(_accountDashbaoardModel);

            // Assert
            panelActionResult.Should().Be("SignAgreement");
        }

        [Test, Category("UnitTest")]
        public void WhenAgreenentHasBeenSigned_ShouldPromptToReserveFunding()
        {
            // Arrange
            _accountDashbaoardModel.AgreementsToSign = false;

            // Act
            var panelActionResult = _sut.GetPanel1Action(_accountDashbaoardModel);

            // Assert
            panelActionResult.Should().Be("CheckFunding");
        }

        [Test, Category("UnitTest")]
        public void WhenPayeSchemeHasNotBeenAdded_ShouldPromptToAddPayeScheme()
        {
            // Arrange
            _accountDashbaoardModel.PayeSchemeCount = 0;

            // Act
            var panelActionResult = _sut.GetPanel1Action(_accountDashbaoardModel);

            // Assert
            panelActionResult.Should().Be("AddPAYE");
        }
    }
}
