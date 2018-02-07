using System;

namespace SFA.DAS.EAS.Application.Messages
{
    public interface IAuthorizedMessage
    {
        Guid? UserExternalId { get; set; }
        string AccountHashedId { get; set; }
    }
}