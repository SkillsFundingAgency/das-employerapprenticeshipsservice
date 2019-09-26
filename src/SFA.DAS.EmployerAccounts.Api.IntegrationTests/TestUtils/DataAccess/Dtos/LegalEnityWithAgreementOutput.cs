namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess.Dtos
{
    public class LegalEnityWithAgreementOutput
    {
        public long LegalEntityId { get; set; }
        public long EmployerAgreementId { get; set; }
        public string HashedAgreementId { get; set; }
    }
}