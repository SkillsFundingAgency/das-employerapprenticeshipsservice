using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Types.Models;

namespace SFA.DAS.EmployerFinance.Extensions
{
    public static class ExpireFundsExtensions
    {
        public static Dictionary<CalendarPeriod, decimal> ToCalendarPeriodDictionary(this IEnumerable<LevyFundsIn> levyFundsIn)
        {
            return levyFundsIn.ToDictionary(fund => new CalendarPeriod(fund.CalendarPeriodYear, fund.CalendarPeriodMonth), fund => fund.FundsIn);
        }

        public static Dictionary<CalendarPeriod, decimal> ToCalendarPeriodDictionary(this IEnumerable<PaymentFundsOut> paymentFundsOut)
        {
            return paymentFundsOut.ToDictionary(fund => new CalendarPeriod(fund.CalendarPeriodYear, fund.CalendarPeriodMonth), fund => fund.FundsOut);
        }
    }
}
