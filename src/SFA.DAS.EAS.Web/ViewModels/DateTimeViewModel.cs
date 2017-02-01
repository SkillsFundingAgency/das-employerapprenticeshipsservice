using System;
using System.Globalization;
using System.Threading;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public sealed class DateTimeViewModel
    {
        private const int NinetyYearsInTheFuture = 90;
        private int? _year;

        public DateTimeViewModel() : this(NinetyYearsInTheFuture)
        {
        }

        /// <summary>
        /// View Model representing a date entry field
        /// </summary>
        /// <param name="date"></param>
        /// <param name="twoDigitMaxYear">Optional: Number of years from current year where to pivot 2 digit year dates</param>
        public DateTimeViewModel(DateTime? date, int twoDigitMaxYear = NinetyYearsInTheFuture) : this(twoDigitMaxYear)
        {
            Day = date?.Day;
            Month = date?.Month;
            Year = date?.Year;
        }

        /// <summary>
        /// View Model representing a date entry field
        /// </summary>
        /// <param name="day"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="twoDigitMaxYear">Optional: Number of years from current year where to pivot 2 digit year dates</param>
        public DateTimeViewModel(int? day, int? month, int? year, int twoDigitMaxYear = NinetyYearsInTheFuture) : this(twoDigitMaxYear)
        {
            Day = day;
            Month = month;
            Year = year;
        }

        /// <summary>
        /// View Model representing a date entry field
        /// </summary>
        /// <param name="twoDigitMaxYear">Optional: Number of years from current year where to pivot 2 digit year dates</param>
        public DateTimeViewModel(int twoDigitMaxYear = NinetyYearsInTheFuture)
        {
            MaxYear = System.DateTime.Now.Year + twoDigitMaxYear;
        }

        public DateTime? DateTime => ToDateTime();

        public int? Day { get; set; }

        public int? Month { get; set; }

        public int? Year
        {
            get
            {
                return _year;
            }
            set
            {
                if (value < 100)
                {
                    var culture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
                    culture.DateTimeFormat.Calendar.TwoDigitYearMax = MaxYear;

                    DateTime dateTimeOut;
                    _year = System.DateTime.TryParseExact(
                        $"{value.Value.ToString("00")}-1-1",
                        "yy-M-d",
                        culture,
                        DateTimeStyles.None,
                        out dateTimeOut) ? dateTimeOut.Year : value;
                }
                else
                {
                    _year = value;
                }
            }
        }

        private int MaxYear { get; }

        private DateTime? ToDateTime()
        {
            return CreateDateTime(Day ?? 1, Month, Year);
        }

        private DateTime? CreateDateTime(int? day, int? month, int? year)
        {
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                DateTime dateTimeOut;
                if (System.DateTime.TryParseExact(
                    $"{year.Value}-{month.Value}-{day.Value}",
                    "yyyy-M-d",
                    CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTimeOut))
                {
                    return dateTimeOut;
                }
            }

            return null;
        }
    }
}