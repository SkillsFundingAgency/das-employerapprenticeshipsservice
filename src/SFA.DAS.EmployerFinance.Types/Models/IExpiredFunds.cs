using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Types.Models
{
    public interface IExpiredFunds
    {
        decimal GetExpiringFundsByDate(IDictionary<CalendarPeriod, decimal> fundsIn, IDictionary<CalendarPeriod, decimal> fundsOut, DateTime date, IDictionary<CalendarPeriod, decimal> expired, int expiryPeriod);
        IDictionary<CalendarPeriod, decimal> GetExpiringFunds(IDictionary<CalendarPeriod, decimal> fundsIn, IDictionary<CalendarPeriod, decimal> fundsOut, IDictionary<CalendarPeriod, decimal> expired, int expiryPeriod);
    }
}
