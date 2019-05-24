using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client.Models.Concrete;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Helpers
{
    [TestFixture]
    public class WhenRequestingTheCallToActionPanel
    {
        private INextActionPanelViewHelper _sut;
        private AccountDashboardViewModel _accountDashboardModel;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _sut = new NextActionPanelViewHelper();
        }

        [SetUp]
        public void SetUp()
        {
            _accountDashboardModel = new AccountDashboardViewModel
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
            var nextAction = _sut.GetNextAction(_accountDashboardModel);

            // Assert
            nextAction.ViewName.Should().Be("SignAgreement");
        }

        [Test, Category("UnitTest")]
        public void WhenAgreenentHasBeenSigned_ShouldPromptToReserveFunding()
        {
            // Arrange
            _accountDashboardModel.AgreementsToSign = false;

            // Act
            var nextAction = _sut.GetNextAction(_accountDashboardModel);

            // Assert
            nextAction.ViewName.Should().Be("CheckFunding");
        }

        [Test, Category("UnitTest")]
        public void WhenPayeSchemeHasNotBeenAdded_ShouldPromptToAddPayeScheme()
        {
            // Arrange
            _accountDashboardModel.PayeSchemeCount = 0;

            // Act
            var nextAction = _sut.GetNextAction(_accountDashboardModel);

            // Assert
            nextAction.ViewName.Should().Be("AddPaye");
        }
    }
}
