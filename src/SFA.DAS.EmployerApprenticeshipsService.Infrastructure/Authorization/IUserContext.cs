using System;

namespace SFA.DAS.EAS.Infrastructure.Authorization
{
    public interface IUserContext
    {
        long Id { get; }
        Guid Ref { get; }
        string Email { get; }
    }
}