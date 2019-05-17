using System.Linq;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Helpers
{
    public class NextActionPanelViewHelper : INextActionPanelViewHelper
    {
        public PanelViewModel<AccountDashboardViewModel> GetNextAction(AccountDashboardViewModel model)
        {
            var viewName = "CheckFunding";
            if (model.PayeSchemeCount == 0)
            {
                viewName = "AddPAYE";
            }
            else
            {
                if (model.AgreementsToSign)
                {
                    viewName = "SignAgreement";
                }
                else if (model.RecentlyAddedReservationId != null
                    || model.AccountViewModel?.AccountLegalEntities?.FirstOrDefault()?.ReservedFundings?.Any() == true)
                {
                    viewName = "FundingComplete";
                    SetFundingComplete(model);
                }
            }

            return new PanelViewModel<AccountDashboardViewModel> { ViewName = viewName, Data = model };
        }

        private static void SetFundingComplete(AccountDashboardViewModel model)
        {
            //todo: no need to return everything in the event in AccountDto, just what we need to display (probably only save what we need to show also)
            //todo: accountDto now mixed concrete/interfaces, which is inconsistent

            if (model.RecentlyAddedReservationId != null)
            {
                var legalEntity = model.AccountViewModel?.AccountLegalEntities
                    ?.FirstOrDefault(ale => ale.ReservedFundings?.Any(rf => rf.ReservationId == model.RecentlyAddedReservationId) == true);

                model.ReservedFundingToShowLegalEntityName = legalEntity?.LegalEntityName;

                // would be better to create new model to contain what the panel needs to show,
                // but we'll be replacing this with displaying all reserved funds anyway
                model.ReservedFundingToShow =
                    legalEntity?.ReservedFundings?.FirstOrDefault(rf =>
                        rf.ReservationId == model.RecentlyAddedReservationId);
            }

            if (model.ReservedFundingToShow == null)
            {
                var legalEntity = model.AccountViewModel?.AccountLegalEntities?.First();
                model.ReservedFundingToShowLegalEntityName = legalEntity?.LegalEntityName;
                model.ReservedFundingToShow = legalEntity?.ReservedFundings?.First();
            }
        }
    }
}