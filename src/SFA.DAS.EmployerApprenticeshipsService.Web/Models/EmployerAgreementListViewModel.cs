using System.Collections.Generic;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Web.Models
{
    public class EmployerAgreementListViewModel
    {
        public long AccountId { get; set; }
        public List<EmployerAgreementView> EmployerAgreements { get; set; }
        public string HashedId { get; set; }
    }
}