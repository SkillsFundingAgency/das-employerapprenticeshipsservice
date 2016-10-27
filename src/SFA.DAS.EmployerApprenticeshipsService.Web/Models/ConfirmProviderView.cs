using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class ConfirmProviderView
    {
        public string HashedAccountId { get; set; }
        public string LegalEntityCode { get; set; }
        public string LegalEntityName { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
        public List<Provider> Providers { get; set; }
    }
}