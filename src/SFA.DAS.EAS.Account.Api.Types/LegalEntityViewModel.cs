using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class LegalEntityViewModel : IAccountResource
    {
        public List<AgreementViewModel> Agreements { get; set; }
        public string Code { get; set; }
        public string DasAccountId { get; set; }
        public DateTime? DateOfInception { get; set; }
        public string Address { get; set; }
        public long LegalEntityId { get; set; }
        public string Name { get; set; }
        public string PublicSectorDataSource { get; set; }
        public string Sector { get; set; }
        public string Source { get; set; }
        public short SourceNumeric { get; set; }
        public string Status { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }

        [Obsolete]
        public string AgreementSignedByName { get; set; }

        [Obsolete]
        public DateTime? AgreementSignedDate { get; set; }

        [Obsolete]
        public EmployerAgreementStatus AgreementStatus { get; set; }
    }
}