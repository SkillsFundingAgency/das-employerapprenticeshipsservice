namespace SFA.DAS.EAS.LevyAnalyser
{
    public class PayrollPeriod
    {
        public PayrollPeriod(string payrollYear, byte payrollMonth)
        {
            PayrollYear = payrollYear;
            PayrollMonth = payrollMonth;
        }

        public string PayrollYear { get; }
        public byte PayrollMonth { get; }
    }
}