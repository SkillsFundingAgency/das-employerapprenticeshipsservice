using System;

namespace SFA.DAS.EAS.Application.Messages
{
    public interface IAuthorizedMessage
    {
        long? AccountId { get; set; }
        string AccountHashedId { get; set; }
        string AccountPublicHashedId { get; set; }
        long? UserId { get; set; }
        Guid? UserExternalId { get; set; }
    }
}