using System;

namespace SFA.DAS.EAS.Domain.Models.Date
{
    public abstract class MonthYearDateTime
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public bool ValidMonth => Month >= 1 && Month <= 12;

        public bool ValidYear => Year >= 1 && Year <= 9999;

        public virtual bool Valid => ValidMonth && ValidYear;

        public bool DateInFuture
        {
            get
            {
                var maximumDate = DateTime.Today;

                if (Year > maximumDate.Year)
                    return true;

                if (Year == maximumDate.Year && Month > maximumDate.Month)
                    return true;

                return false;
            }
        }

        public abstract DateTime ToDateTime();
    }
}
