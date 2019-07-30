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
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.NoneLevyExpressionOfInterest, false)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Levy, true)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Inconsistent, false)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Unknown, false)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.NoneLevyExpressionOfInterest, false)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Levy, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Inconsistent, false)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Unknown, false)]
        public void ShowManageYourLevyLink_GivenValues_ReturnsExpectedResult(
            ApprenticeshipEmployerType apprenticeshipEmployerType,
            AgreementType agreementType,
            bool expectedValue)
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                ApprenticeshipEmployerType = apprenticeshipEmployerType,
                AgreementInfo = new AgreementInfoViewModel
                {
                    Type = agreementType
                }
            };

            // Act
            var result = AccountDashboardViewModelExtensions.ShowManageYourLevyLink(model);

            // Assert
            Assert.AreEqual(expectedValue, result);
        }

        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.NoneLevyExpressionOfInterest, false)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Levy, false)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Inconsistent, false)]
        [TestCase(ApprenticeshipEmployerType.Levy, AgreementType.Unknown, false)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.NoneLevyExpressionOfInterest, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Levy, false)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Inconsistent, false)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AgreementType.Unknown, false)]
        public void ShowYourFundingReservationsLink_GivenValues_ReturnsExpectedResult(
           ApprenticeshipEmployerType apprenticeshipEmployerType,
           AgreementType agreementType,
           bool expectedValue)
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                ApprenticeshipEmployerType = apprenticeshipEmployerType,
                AgreementInfo = new AgreementInfoViewModel
                {
                    Type = agreementType
                }
            };

            // Act
            var result = AccountDashboardViewModelExtensions.ShowYourFundingReservationsLink(model);

            // Assert
            Assert.AreEqual(expectedValue, result);
        }
    }
}
