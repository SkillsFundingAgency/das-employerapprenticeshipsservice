using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class ConfirmOrganisationToRemoveViewModel : ViewModelBase
    {
        public string Name { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public string HashedAccountId { get; set; }
        public int? RemoveOrganisation { get; set; }
        public string RemoveOrganisationError => GetErrorMessage(nameof(RemoveOrganisation));
        public bool HasSignedAgreement { get; set; }
    }
}