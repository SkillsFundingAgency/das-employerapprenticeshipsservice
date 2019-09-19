namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public class HasAgreementBeenSignedRequest
    {
        public long AccountId { get; set; }
        public string AgreementType { get; set; }
        public int AgreementVersion { get; set; }
    }
}