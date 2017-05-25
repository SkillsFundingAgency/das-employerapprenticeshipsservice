using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Web.ViewModels.Organisation
{
    public class LegalAgreementsToRemoveViewModel : ViewModelBase
    {
        public List<RemoveEmployerAgreementView> Agreements { get; set; }
    }
}