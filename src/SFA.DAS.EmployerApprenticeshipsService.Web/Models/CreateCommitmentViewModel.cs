namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public sealed class SelectLegalEntityViewModel
    {
        public string LegalEntityCode { get; set; }
    }

    public sealed class SelectProviderViewModel
    {
        public string LegalEntityCode { get; set; }

        public string ProviderId { get; set; }
    }

    public sealed class CreateCommitmentViewModel
    {
        public string Name { get; set; }
        public string HashedAccountId { get; set; }
        public string LegalEntityCode { get; set; }
        public string LegalEntityName { get; set; }
        public long ProviderId { get; set; }
        public string ProviderName { get; set; }    
    }
}