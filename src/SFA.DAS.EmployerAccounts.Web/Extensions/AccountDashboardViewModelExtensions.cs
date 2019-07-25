using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Types.Models.Agreement;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class AccountDashboardViewModelExtensions
    {
        public static bool ShowManageYourLevyLink(this AccountDashboardViewModel model)
        {
            if((model.AgreementInfo.Type == AgreementType.NonLevy && model.AgreementInfo.Phase?.Type == PhaseType.EOI))
            {
                return false;
            }

            return true;
        }

        public static bool ShowYourFundingReservationsLink(this AccountDashboardViewModel model)
        {
            if(
                model.ApprenticeshipEmployerType == ApprenticeshipEmployerType.NonLevy || 
                (model.AgreementInfo.Type == AgreementType.NonLevy && model.AgreementInfo.Phase?.Type == PhaseType.EOI))
            {
                return true;
            }

            return false;
        }
    }
}