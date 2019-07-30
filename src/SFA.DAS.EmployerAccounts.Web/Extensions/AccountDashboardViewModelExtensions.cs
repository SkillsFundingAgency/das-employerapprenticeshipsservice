using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Types.Models.Agreement;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using AgreementType = SFA.DAS.EmployerAccounts.Types.Models.Agreement.AgreementType;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class AccountDashboardViewModelExtensions
    {
        public static bool ShowManageYourLevyLink(this AccountDashboardViewModel model)
        {
            if (model.AgreementInfo.Type != AgreementType.NoneLevyExpressionOfInterest)
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