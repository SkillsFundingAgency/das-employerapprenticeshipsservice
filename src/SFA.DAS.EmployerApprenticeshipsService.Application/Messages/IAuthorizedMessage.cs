using System;

namespace SFA.DAS.EAS.Application.Messages
{
    public interface IAuthorizedMessage
    {
        string AccountHashedId { get; set; }
        long? AccountId { get; set; }
        Guid? UserExternalId { get; set; }
        long? UserId { get; set; }
    }
}