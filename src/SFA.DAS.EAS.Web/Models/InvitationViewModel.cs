using System;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Web.Models
{
    public class InvitationViewModel
    {
        public bool IsUser { get; set; }
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public InvitationStatus Status { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}