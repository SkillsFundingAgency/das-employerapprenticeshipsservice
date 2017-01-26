using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class LegalEntityViewModel : IAccountResource
    {
        public long Id { get; set; }
        public string RegisteredAddress { get; set; }
        public string Source { get; set; }
        public string CompanyStatus { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime? DateOfIncorporation { get; set; }
    }
}