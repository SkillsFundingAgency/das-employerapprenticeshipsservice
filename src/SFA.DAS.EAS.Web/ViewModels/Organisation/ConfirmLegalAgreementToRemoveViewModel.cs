using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Web.ViewModels.Organisation
{
    public class ConfirmLegalAgreementToRemoveViewModel : ViewModelBase
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public string HashedAgreementId { get; set; }
        public string HashedAccountId { get; set; }
        public int? RemoveOrganisation { get; set; }

        public string RemoveOrganisationError => GetErrorMessage(nameof(RemoveOrganisation));
        public EmployerAgreementStatus AgreementStatus { get; set; }
    }
}