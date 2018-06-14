using System;
using SFA.DAS.EAS.Infrastructure.Interfaces;
using SFA.DAS.EAS.Infrastructure.Models;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class FinancialYearDateCalculator : IFinancialYearDateCalculator
    {
        private readonly FinancialYearCutoff _financialYearCutoff;

        public FinancialYearDateCalculator(FinancialYearCutoff fincFinancialYearCutoff)
        {
            _financialYearCutoff = fincFinancialYearCutoff;
        }

        public short GetEndFinancialYear(DateTime atPointInTime)
        {
            var month = (short)atPointInTime.Month;
            var year = (short)atPointInTime.Year;

            if (month > _financialYearCutoff.Month || (month == _financialYearCutoff.Month && atPointInTime.Day >= _financialYearCutoff.Day))
            {
                return (short) (year + 1);
            }

            return year;
        }

        public DateTime GetYearStart(int endYear)
        {
            return new DateTime(endYear - 1, _financialYearCutoff.Month, _financialYearCutoff.Day);
        }

        public DateTime GetYearEnd(int endYear)
        {
            return new DateTime(endYear, _financialYearCutoff.Month, _financialYearCutoff.Day).AddMilliseconds(-1);
        }
    }
}