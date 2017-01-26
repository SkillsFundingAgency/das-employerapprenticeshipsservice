using System.Collections.Generic;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Web.Models
{
    public class EmployerAgreementListViewModel
    {
        public long AccountId { get; set; }
        public List<EmployerAgreementView> EmployerAgreements { get; set; }
        public string HashedAccountId { get; set; }
    }
}