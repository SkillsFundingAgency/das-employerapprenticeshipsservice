namespace SFA.DAS.EAS.Domain
{
    public class PayeView
    {
        public string EmpRef { get; set; }
        public int AccountId { get; set; }
        public string LegalEntityName { get; set; } 
        public long LegalEntityId { get; set; }
    }
}