using System;

namespace SFA.DAS.EAS.Application.Services
{
    public interface IMembershipService
    {
        void ValidateAccountMembership(string accountHashedId, Guid userExternalId);
    }
}