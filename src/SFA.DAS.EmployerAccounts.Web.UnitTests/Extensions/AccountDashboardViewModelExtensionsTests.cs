using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions
{
    [TestFixture]
    public class AccountDashboardViewModelExtensionsTests
    {
        [TestCase(ApprenticeshipEmployerType.Levy, AccountAgreementType.NonLevyExpressionOfInterest, false)]
        [TestCase(ApprenticeshipEmployerType.Levy, AccountAgreementType.Levy, false)]
        [TestCase(ApprenticeshipEmployerType.Levy, AccountAgreementType.Inconsistent, false)]
        [TestCase(ApprenticeshipEmployerType.Levy, AccountAgreementType.Unknown, false)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AccountAgreementType.NonLevyExpressionOfInterest, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AccountAgreementType.Levy, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AccountAgreementType.Inconsistent, true)]
        [TestCase(ApprenticeshipEmployerType.NonLevy, AccountAgreementType.Unknown, true)]
        public void ShowYourFundingReservationsLink_GivenValues_ReturnsExpectedResult(
           ApprenticeshipEmployerType apprenticeshipEmployerType,
           AccountAgreementType agreementType,
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
