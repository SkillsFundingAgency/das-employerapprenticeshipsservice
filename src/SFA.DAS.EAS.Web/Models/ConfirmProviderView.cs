using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.EAS.Web.Models
{
    public class ConfirmProviderView
    {
        public string CohortRef { get; set; }
        public string HashedAccountId { get; set; }
        public string LegalEntityCode { get; set; }
        public string LegalEntityName { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
        public IList<Provider> Providers { get; set; }

        [Required(ErrorMessage = "Select a training provider")]
        public bool? Confirmation { get; set; }
    }
}