using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.UserProfile;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IUserAornPayeLockService
    {
        Task UpdateUserAornPayeAttempt(Guid userRef, bool success);
        Task<UserAornPayeStatus> UserAornPayeStatus(Guid userRef);
    }
}