using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class Invitation
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Email { get; set; }
        public InvitationStatus Status { get; set; }
    }
}