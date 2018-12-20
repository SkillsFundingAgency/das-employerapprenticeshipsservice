using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class LegalAgreementsToRemoveViewModel : ViewModelBase
    {
        public List<RemoveEmployerAgreementView> Agreements { get; set; }
    }
}