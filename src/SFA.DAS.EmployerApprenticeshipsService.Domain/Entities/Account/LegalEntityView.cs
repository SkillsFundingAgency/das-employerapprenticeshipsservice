using System;

namespace SFA.DAS.EAS.Domain.Entities.Account
{
    public class LegalEntityView
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string RegisteredAddress { get; set; }
        public DateTime? DateOfIncorporation { get; set; }
        public string CompanyStatus { get; set; }
        public string Source { get; set; }
    }
}
