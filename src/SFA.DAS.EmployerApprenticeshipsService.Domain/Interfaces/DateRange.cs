using System;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public class DateRange
    {
        /// <summary>
        ///     The first instant of the date range
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     The last instance of the date range
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}