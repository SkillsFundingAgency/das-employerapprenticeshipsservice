using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Types.Models.Agreement;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions
{
    [TestFixture]
    public class AccountDashboardViewModelExtensionsTests
    {
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.NonLevy, PhaseType.EOI, false)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.NonLevy, PhaseType.Default, true)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Levy, PhaseType.EOI, true)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Levy, PhaseType.Default, true)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Inconsistent, PhaseType.EOI, true)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Inconsistent, PhaseType.Default, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.NonLevy, PhaseType.EOI, false)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.NonLevy, PhaseType.Default, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Levy, PhaseType.EOI, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Levy, PhaseType.Default, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Inconsistent, PhaseType.EOI, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Inconsistent, PhaseType.Default, true)]
        public void ShowManageYourLevyLink_GivenValues_ReturnsExpectedResult(
            ApprenticeshipEmployerType apprenticeshipEmployerType,
            AgreementType agreementType,
            PhaseType phaseType,
            bool expectedValue)
        {

            // Arrange
            var model = new AccountDashboardViewModel
            {
                ApprenticeshipEmployerType = apprenticeshipEmployerType,
                AgreementInfo = new AgreementInfoViewModel
                {
                    Type = agreementType,
                    Phase = new AgreementPhase
                    {
                        Type = phaseType
                    }
                }
            };

            // Act
            var result = AccountDashboardViewModelExtensions.ShowManageYourLevyLink(model);

            // Assert
            Assert.AreEqual(expectedValue, result);
        }

        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.NonLevy, PhaseType.EOI, true)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.NonLevy, PhaseType.Default, false)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Levy, PhaseType.EOI, false)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Levy, PhaseType.Default, false)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Inconsistent, PhaseType.EOI, false)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Inconsistent, PhaseType.Default, false)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.NonLevy, PhaseType.EOI, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.NonLevy, PhaseType.Default, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Levy, PhaseType.EOI, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Levy, PhaseType.Default, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Inconsistent, PhaseType.EOI, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Inconsistent, PhaseType.Default, true)]
        public void ShowYourFundingReservationsLink_GivenValues_ReturnsExpectedResult(
           ApprenticeshipEmployerType apprenticeshipEmployerType,
           AgreementType agreementType,
           PhaseType phaseType,
           bool expectedValue)
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                ApprenticeshipEmployerType = apprenticeshipEmployerType,
                AgreementInfo = new AgreementInfoViewModel
                {
                    Type = agreementType,
                    Phase = new AgreementPhase
                    {
                        Type = phaseType
                    }
                }
            };

            // Act
            var result = AccountDashboardViewModelExtensions.ShowYourFundingReservationsLink(model);

            // Assert
            Assert.AreEqual(expectedValue, result);
        }
    }
}
