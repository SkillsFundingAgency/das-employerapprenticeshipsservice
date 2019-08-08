using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class AccountDashboardViewModelExtensions
    {
        public static bool ShowManageYourLevyLink(this AccountDashboardViewModel model)
        {
            if (model.AgreementInfo.Type != AccountAgreementType.NonLevyExpressionOfInterest)
            {
                return true;
            }

            return false;
        }

        public static bool ShowYourFundingReservationsLink(this AccountDashboardViewModel model)
        {
            if(model.ApprenticeshipEmployerType == ApprenticeshipEmployerType.NonLevy)
            {
                return true;
            }

            return false;
        }
    }
}