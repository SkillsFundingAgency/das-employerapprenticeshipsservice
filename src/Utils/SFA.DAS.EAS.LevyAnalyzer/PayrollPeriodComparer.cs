using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.LevyAnalyser
{
    public class PayrollPeriodComparer : IEqualityComparer<PayrollPeriod>
    {
        public bool Equals(PayrollPeriod x, PayrollPeriod y)
        {
            return string.Equals(x.PayrollYear, y.PayrollYear, StringComparison.InvariantCultureIgnoreCase) && x.PayrollMonth == y.PayrollMonth;
        }

        public int GetHashCode(PayrollPeriod x)
        {
            return x.PayrollYear.GetHashCode() +  x.PayrollMonth.GetHashCode();
        }
    }
}