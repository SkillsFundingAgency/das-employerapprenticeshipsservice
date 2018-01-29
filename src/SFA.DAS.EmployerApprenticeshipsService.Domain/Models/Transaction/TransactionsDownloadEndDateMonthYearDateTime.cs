using System;
using SFA.DAS.EAS.Domain.Models.Date;

namespace SFA.DAS.EAS.Domain.Models.Transaction
{
    public class TransactionsDownloadEndDateMonthYearDateTime : MonthYearDateTime
    {
        public DateTime MaximumDate => DateTime.Today;

        public override DateTime ToDateTime()
        {
            if (!ValidMonth)
                throw new InvalidOperationException("This object has an invalid EndDate.Month");

            if (!ValidYear)
                throw new InvalidOperationException("This object has an invalid EndDate.Year");

            if (DateInFuture)
                throw new InvalidOperationException("This EndDate is in the future");

            // return 00:00 on the first day of the next month
            return new DateTime(Year, Month + 1, 1);
        }

        public override bool Valid => ValidMonth && ValidYear & !DateInFuture;

    }
}