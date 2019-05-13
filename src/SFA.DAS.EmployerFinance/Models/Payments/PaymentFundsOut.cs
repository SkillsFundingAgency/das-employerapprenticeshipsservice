namespace SFA.DAS.EmployerFinance.Models.Payments
{
    public class PaymentFundsOut
    {
        public int CalendarPeriodYear { get; set; }
        public int CalendarPeriodMonth { get; set; }
        public decimal FundsOut { get; set; }
    }
}