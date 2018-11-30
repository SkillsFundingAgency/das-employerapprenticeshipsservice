namespace SFA.DAS.EmployerFinance.Messages.Commands
{
    public class ImportAccountPaymentsCommand 
    {
        public long AccountId { get; set; }
        public string PeriodEndRef { get; set; }
    }
}
