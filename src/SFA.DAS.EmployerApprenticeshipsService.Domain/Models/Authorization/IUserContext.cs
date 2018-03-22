using System;

namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public interface IUserContext
    {
        long Id { get; }
        Guid ExternalId { get; }
        string Email { get; }
    }
}