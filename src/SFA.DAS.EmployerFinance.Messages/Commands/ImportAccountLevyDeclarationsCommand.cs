namespace SFA.DAS.EmployerFinance.Messages.Commands
{
    public class ImportAccountLevyDeclarationsCommand 
    {
        public long AccountId { get; set; }
        public string PayeRef { get; set; }
    }
}
