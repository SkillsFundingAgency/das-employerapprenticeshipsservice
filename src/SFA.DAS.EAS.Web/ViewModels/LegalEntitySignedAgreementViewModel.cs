using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class LegalEntitySignedAgreementViewModel
    {
        public string HashedAccountId { get; set; }
        public string LegalEntityCode { get; set; }
        public string CohortRef { get; set; }
        public bool HasSignedAgreement { get; set; }
    }
}