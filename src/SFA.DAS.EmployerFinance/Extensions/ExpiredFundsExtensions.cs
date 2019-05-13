using System.Collections.Generic;
using System.Data;
using SFA.DAS.EmployerFinance.Models.ExpiredFunds;

namespace SFA.DAS.EmployerFinance.Extensions
{
    public static class ExpiredFundsExtensions
    {
        public static DataTable ToExpiredFundsDataTable(this IEnumerable<ExpiredFund> expiredFunds)
        {
            var expiredFundsDataTable = new DataTable();

            expiredFundsDataTable.Columns.Add("CalendarPeriodYear", typeof(int));
            expiredFundsDataTable.Columns.Add("CalendarPeriodMonth", typeof(int));
            expiredFundsDataTable.Columns.Add("Amount", typeof(decimal));

            foreach (var expiredFund in expiredFunds)
            {
                expiredFundsDataTable.Rows.Add(
                    expiredFund.CalendarPeriodYear,
                    expiredFund.CalendarPeriodMonth,
                    expiredFund.Amount);
            }

            return expiredFundsDataTable;
        }
    }
}
