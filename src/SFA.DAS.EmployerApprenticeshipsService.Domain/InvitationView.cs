using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class InvitationView
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public InvitationStatus Status { get; set; }
        public int InternalUserId { get; set; }
        public Guid ExternalUserId { get; set; }
    }
}