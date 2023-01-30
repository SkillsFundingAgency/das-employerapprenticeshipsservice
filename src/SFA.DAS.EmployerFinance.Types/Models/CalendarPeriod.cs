using System;

namespace SFA.DAS.EmployerFinance.Types.Models
{
    public class CalendarPeriod : IComparable<CalendarPeriod>, IEquatable<CalendarPeriod>
    {
        public CalendarPeriod(int year, int month)
        {
            Year = year;
            Month = month;
        }

        public int Year { get; }
        public int Month { get; }

        public int CompareTo(CalendarPeriod compareTo)
        {
            return Compare(this, compareTo);
        }

        public static bool operator >(CalendarPeriod period1, CalendarPeriod period2)
        {
            return Compare(period1, period2) > 0;
        }

        public static bool operator <(CalendarPeriod period1, CalendarPeriod period2)
        {
            return Compare(period1, period2) < 0;
        }

        public static bool operator <=(CalendarPeriod period1, CalendarPeriod period2)
        {
            return Compare(period1, period2) <= 0;
        }

        public static bool operator >=(CalendarPeriod period1, CalendarPeriod period2)
        {
            return Compare(period1, period2) >= 0;
        }

        public static bool operator ==(CalendarPeriod period1, CalendarPeriod period2)
        {
            return Equals(period1, period2);
        }

        public static bool operator !=(CalendarPeriod period1, CalendarPeriod period2)
        {
            return !Equals(period1, period2);
        }

        public bool AreSameTaxYear(CalendarPeriod compareTo)
        {
            return CheckPeriodsAreInSameTaxYear(new DateTime(Year, Month, 1), new DateTime(compareTo.Year, compareTo.Month, 1));
        }

        private static bool CheckPeriodsAreInSameTaxYear(DateTime firstPeriod, DateTime secondPeriod)
        {

            var taxYearFirstMonth = GetTaxYearFromTransactionDate(firstPeriod);
            var taxYearSecondMonth = GetTaxYearFromTransactionDate(secondPeriod);
            var startPeriodTaxYear = GetTaxYearFromDate(taxYearFirstMonth);

            var endPeriodTaxYear = GetTaxYearFromDate(taxYearSecondMonth);

            return startPeriodTaxYear == endPeriodTaxYear;
        }

        private static DateTime GetTaxYearFromTransactionDate(DateTime transactionDate)
        {
            return transactionDate.AddMonths(-1);
        }

        private static int GetTaxYearFromDate(DateTime firstPeriod)
        {
            return firstPeriod.Month >= 1 && firstPeriod.Month < 4
                ? firstPeriod.Year - 1
                : firstPeriod.Year;
        }

        private static int Compare(CalendarPeriod calendarPeriod1, CalendarPeriod calendarPeriod2)
        {
            if (calendarPeriod1 == null || calendarPeriod2 == null)
            {
                return 0;
            }

            if (calendarPeriod1.Year > calendarPeriod2.Year)
            {
                return 1;
            }

            if (calendarPeriod1.Year < calendarPeriod2.Year)
            {
                return -1;
            }

            if (calendarPeriod1.Year == calendarPeriod2.Year)
            {
                if (calendarPeriod1.Month > calendarPeriod2.Month)
                {
                    return 1;
                }

                if (calendarPeriod1.Month < calendarPeriod2.Month)
                {
                    return -1;
                }
            }

            return 0;
        }

        public bool Equals(CalendarPeriod other)
        {
            return other != null && other.Year == Year && other.Month == Month;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CalendarPeriod);
        }

        public override int GetHashCode()
        {
            return Year.GetHashCode() ^ Month.GetHashCode();
        }
    }
}