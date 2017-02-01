using System;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Web.ViewModels
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