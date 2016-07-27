using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class TeamMember
    {
        public bool IsUser { get; set; }
        public int Id { get; set; }

        public string Email { get; set; }
        public string UserRef { get; set; }
        public int AccountId { get; set; }
        public Role Role { get; set; }

        public InvitationStatus Status { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
