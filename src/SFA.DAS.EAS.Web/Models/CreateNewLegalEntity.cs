using System;

namespace SFA.DAS.EAS.Web.Models
{
    public class CreateNewLegalEntity
    {
        public string HashedAccountId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public DateTime IncorporatedDate { get; set; }
        public bool UserIsAuthorisedToSign { get; set; }
        public bool SignedAgreement { get; set; }
        public DateTime SignedDate { get; set; }
        public string ExternalUserId { get; set; }

    }
}