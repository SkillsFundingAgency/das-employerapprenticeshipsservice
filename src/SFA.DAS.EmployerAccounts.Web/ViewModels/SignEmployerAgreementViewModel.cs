using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class SignEmployerAgreementViewModel
    {
        public EmployerAgreementView EmployerAgreement { get; set; }
        public EmployerAgreementView PreviouslySignedEmployerAgreement { get; set; }
        public bool NoChoiceSelected { get; set; }
        public int LegalEntitiesCount { get; set; }
    }
}