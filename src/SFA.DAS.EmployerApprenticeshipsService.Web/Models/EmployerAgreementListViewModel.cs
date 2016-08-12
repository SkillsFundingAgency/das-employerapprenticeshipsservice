using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class EmployerAgreementListViewModel
    {
        public long AccountId { get; set; }
        public List<EmployerAgreementView> EmployerAgreements { get; set; }
    }
}