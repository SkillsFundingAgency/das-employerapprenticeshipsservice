using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerFinance.Models.ExpiredFunds;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Types.Models;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.EventHandlers
{
    public static class ExpiredFundsComparisonHelper
    {
        public static bool AreFundsInEqual(IEnumerable<LevyFundsIn> fundsInList, IDictionary<CalendarPeriod, decimal> expiredFundsDictionary)
        {
            if (fundsInList.Count() != expiredFundsDictionary.Count)
            {
                return false;
            }

            foreach (var fund in fundsInList)
            {
                if (!expiredFundsDictionary.Any(x =>
                    x.Key.Year == fund.CalendarPeriodYear &&
                    x.Key.Month == fund.CalendarPeriodMonth &&
                    x.Value == fund.FundsIn))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AreFundsOutEqual(IEnumerable<PaymentFundsOut> fundsOutList, IDictionary<CalendarPeriod, decimal> expiredFundsDictionary)
        {
            if (fundsOutList.Count() != expiredFundsDictionary.Count)
            {
                return false;
            }

            foreach (var fund in fundsOutList)
            {
                if (!expiredFundsDictionary.Any(x =>
                    x.Key.Year == fund.CalendarPeriodYear &&
                    x.Key.Month == fund.CalendarPeriodMonth &&
                    x.Value == fund.FundsOut))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AreExpiredFundsEqual(IEnumerable<ExpiredFund> expiredFundsEntityList, IDictionary<CalendarPeriod, decimal> expiredFundsDictionary)
        {
            if (expiredFundsEntityList.Count() != expiredFundsDictionary.Count)
            {
                return false;
            }

            foreach (var fund in expiredFundsEntityList)
            {
                if (!expiredFundsDictionary.Any(x =>
                    x.Key.Year == fund.CalendarPeriodYear &&
                    x.Key.Month == fund.CalendarPeriodMonth &&
                    x.Value == fund.Amount))
                {
                    return false;
                }
            }

            return true;
        }
    }
}