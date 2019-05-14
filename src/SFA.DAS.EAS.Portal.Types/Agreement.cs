namespace SFA.DAS.EAS.Portal.Types
{
    public class Agreement
    {
        public string HashedAgreementId { get; set; }
        public int Version { get; set; }
        public bool IsPending { get; set; }
    }
}