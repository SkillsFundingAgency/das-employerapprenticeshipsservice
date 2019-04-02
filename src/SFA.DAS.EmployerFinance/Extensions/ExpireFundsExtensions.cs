using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerFinance.Models.ExpiredFunds;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Types.Models;

namespace SFA.DAS.EmployerFinance.Extensions
{
    public static class ExpireFundsExtensions
    {
        public static IDictionary<CalendarPeriod, decimal> ToCalendarPeriodDictionary(this IEnumerable<LevyFundsIn> levyFundsIn)
        {
            return levyFundsIn.ToDictionary(fund => new CalendarPeriod(fund.CalendarPeriodYear, fund.CalendarPeriodMonth), fund => fund.FundsIn);
        }

        public static IDictionary<CalendarPeriod, decimal> ToCalendarPeriodDictionary(this IEnumerable<PaymentFundsOut> paymentFundsOut)
        {
            return paymentFundsOut.ToDictionary(fund => new CalendarPeriod(fund.CalendarPeriodYear, fund.CalendarPeriodMonth), fund => fund.FundsOut);
        }

        public static IDictionary<CalendarPeriod, decimal> ToCalendarPeriodDictionary(this IEnumerable<ExpiredFund> expiredFunds)
        {
            return expiredFunds.ToDictionary(fund => new CalendarPeriod(fund.CalendarPeriodYear, fund.CalendarPeriodMonth), fund => fund.Amount);
        }

        public static IEnumerable<ExpiredFund> ToExpiredFundsList(this IDictionary<CalendarPeriod, decimal> calendarPeriodDictionary)
        {
            return calendarPeriodDictionary.Select(x => new ExpiredFund{ Amount = x.Value, CalendarPeriodYear = x.Key.Year, CalendarPeriodMonth = x.Key.Month });
        }
    }
}
