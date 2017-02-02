using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class LegalEntityViewModel : IAccountResource
    {
        public string DasAccountId { get; set; }
        public long LegalEntityId { get; set; }
        public string Address { get; set; }
        public string Source { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime? DateOfInception { get; set; }
        public string PublicSectorDataSource { get; set; }
    }
}