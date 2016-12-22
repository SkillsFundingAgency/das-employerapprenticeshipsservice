using System;
using System.Globalization;

namespace SFA.DAS.EAS.Web.Models.Types
{
    public sealed class DateTimeViewModel
    {
        public DateTimeViewModel()
        {
        }

        public DateTimeViewModel(DateTime? date)
        {
            Day = date?.Day;
            Month = date?.Month;
            Year = date?.Year;
        }

        public DateTimeViewModel(int? day, int? month, int? year)
        {
            Day = day;
            Month = month;
            Year = year;
        }

        public DateTime? DateTime => ToDateTime();

        public int? Day { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }

        private DateTime? ToDateTime()
        {
            var year = Year;
            if (year.HasValue && year.Value < 100 && year.Value > -1)
                year = CultureInfo.InvariantCulture.Calendar.ToFourDigitYear(year.Value);

            return CreateDateTime(Day ?? 1, Month, year);
        }

        private DateTime? CreateDateTime(int? day, int? month, int? year)
        {
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                DateTime dateOfBirthOut;
                if (System.DateTime.TryParseExact(
                    $"{year.Value}-{month.Value}-{day.Value}",
                    "yyyy-M-d",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirthOut))
                {
                    return dateOfBirthOut;
                }
            }

            return null;
        }
    }
}