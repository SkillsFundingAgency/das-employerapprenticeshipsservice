using System;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public interface IMembershipContext
    {
        long AccountId { get; }
        string AccountHashedId { get; }
        long UserId { get; }
        Guid UserExternalId { get; }
        string UserEmail { get; }
        Role Role { get; }
    }
}