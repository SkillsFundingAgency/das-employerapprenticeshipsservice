﻿using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class AccountDashboardViewModelExtensions
    {
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