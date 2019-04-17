using System;
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
        public static IDictionary<CalendarPeriod, decimal> GetExpiredFunds(this IExpiredFunds expiredFundsService, IDictionary<CalendarPeriod, decimal> fundsIn, IDictionary<CalendarPeriod, decimal> fundsOut, IDictionary<CalendarPeriod, decimal> expired, int expiryPeriod, DateTime today)
        {
            var currentCalendarPeriod = new CalendarPeriod(today.Year, today.Month);
            var expiringFunds = expiredFundsService.GetExpiringFunds(fundsIn, fundsOut, expired, expiryPeriod);

            var expiredFunds = expiringFunds
                .Where(ef => ef.Key <= currentCalendarPeriod && ef.Value > 0 && !expired.Any(e => e.Key == ef.Key && e.Value == ef.Value))
                .ToDictionary(e => e.Key, e => e.Value);

            return expiredFunds;
        }

        public static IDictionary<CalendarPeriod, decimal> ToCalendarPeriodDictionary(this IEnumerable<LevyFundsIn> levyFundsIn)
        {
            return levyFundsIn.ToDictionary(fund => new CalendarPeriod(fund.CalendarPeriodYear, fund.CalendarPeriodMonth), fund => fund.FundsIn);
        }

        public static IDictionary<CalendarPeriod, decimal> ToCalendarPeriodDictionary(this IEnumerable<PaymentFundsOut> paymentFundsOut)
        {
            return paymentFundsOut.ToDictionary(fund => new CalendarPeriod(fund.CalendarPeriodYear, fund.CalendarPeriodMonth), fund => -fund.FundsOut);
        }

        public static IDictionary<CalendarPeriod, decimal> ToCalendarPeriodDictionary(this IEnumerable<ExpiredFund> expiredFunds)
        {
            return expiredFunds.ToDictionary(fund => new CalendarPeriod(fund.CalendarPeriodYear, fund.CalendarPeriodMonth), fund => -fund.Amount);
        }

        public static IEnumerable<ExpiredFund> ToExpiredFundsList(this IDictionary<CalendarPeriod, decimal> calendarPeriodDictionary)
        {
            return calendarPeriodDictionary.Select(x => new ExpiredFund{ Amount = -x.Value, CalendarPeriodYear = x.Key.Year, CalendarPeriodMonth = x.Key.Month });
        }
    }
}
