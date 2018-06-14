using System;

namespace SFA.DAS.EAS.Infrastructure.Models
{
    public class FinancialYearDetails
    {
        public FinancialYearDetails(int startYear, int endYear, DateTime startTime, DateTime endTime)
        {
            StartYear = startYear;
            EndYear = endYear;
            StartTime = startTime;
            EndTime = endTime;
        }

        public int StartYear { get; }
        public int EndYear { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }

        public override string ToString()
        {
            return $"{StartYear}-{EndYear}";
        }
    }
}