using System;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Domain.Data.Entities.Account
{
    public class LegalEntityView
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfInception { get; set; }
        public string Status { get; set; }
        public string Source { get; set; }
        public string PublicSectorDataSource { get; set; }
        public string Sector { get; set; }

        public EmployerAgreementStatus AgreementStatus { get; set; }
        public string AgreementSignedByName { get; set; }
        public DateTime? AgreementSignedDate { get; set; }
    }
}
