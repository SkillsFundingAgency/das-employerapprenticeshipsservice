using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class ConfirmOrganisationToRemoveViewModel : ViewModelBase
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public string HashedAccountId { get; set; }
        public int? RemoveOrganisation { get; set; }

        public string RemoveOrganisationError => GetErrorMessage(nameof(RemoveOrganisation));
        public EmployerAgreementStatus AgreementStatus { get; set; }
    }
}