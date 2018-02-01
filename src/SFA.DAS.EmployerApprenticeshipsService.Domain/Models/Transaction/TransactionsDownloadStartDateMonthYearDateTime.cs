using System;
using SFA.DAS.EAS.Domain.Models.Date;

namespace SFA.DAS.EAS.Domain.Models.Transaction
{
    public class TransactionsDownloadStartDateMonthYearDateTime : MonthYearDateTime
    {
        public DateTime MaximumDate => DateTime.Today;

        public override DateTime ToDateTime()
        {
            if (!ValidMonth)
                throw new InvalidOperationException("This object has an invalid StartDate.Month");

            if (!ValidYear)
                throw new InvalidOperationException("This object has an invalid StartDate.Year");

            return new DateTime(Year, Month, 1);
        }

        public override bool Valid => ValidMonth && ValidYear;
    }
}