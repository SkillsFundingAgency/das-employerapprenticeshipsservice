using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class CreateNewLegalEntity
    {
        public long AccountId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public DateTime IncorporatedDate { get; set; }
        public bool UserIsAuthorisedToSign { get; set; }
        public bool SignedAgreement { get; set; }
        public string ExternalUserId { get; set; }

    }
}