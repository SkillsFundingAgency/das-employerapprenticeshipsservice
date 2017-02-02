using System;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class CreateNewLegalEntityViewModel
    {
        public string HashedAccountId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public DateTime? IncorporatedDate { get; set; }
        public bool UserIsAuthorisedToSign { get; set; }
        public bool SignedAgreement { get; set; }
        public DateTime SignedDate { get; set; }
        public string ExternalUserId { get; set; }
        public string LegalEntityStatus { get; set; }
        public short Source { get; set; }
        public short? PublicSectorDataSource { get; set; }
    }
}