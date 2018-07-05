namespace SFA.DAS.EAS.Web.ViewModels.Organisation
{
    public class ReviewOrganisationAddressViewModel : OrganisationViewModelBase
    {
        public string HashedAccountId { get; set; }
        public string HashedLegalEntityId { get; set; }
        public string HashedAgreementId { get; set; }
        public string RefreshedName { get; set; }
        public string RefreshedAddress { get; set; }
    }
}