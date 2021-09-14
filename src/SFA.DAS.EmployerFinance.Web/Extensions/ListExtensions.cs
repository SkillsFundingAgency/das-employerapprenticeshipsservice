using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerFinance.Models.Apprenticeships;

namespace SFA.DAS.EmployerFinance.Web.Extensions
{
    public static class ListExtensions
    {
        public static long GetActivelyFundedApprenticeCount(this IEnumerable<ApprenticeshipDetail> apprenticesDetails)
        {
            return apprenticesDetails.Count(o =>
                o.ApprenticeshipStatus == ApprenticeshipStatus.WaitingToStart ||
                o.ApprenticeshipStatus == ApprenticeshipStatus.Live ||
                o.ApprenticeshipStatus == ApprenticeshipStatus.Paused);
        }
    }
}