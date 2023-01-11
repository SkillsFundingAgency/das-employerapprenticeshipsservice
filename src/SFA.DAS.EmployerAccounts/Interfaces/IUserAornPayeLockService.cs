namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IUserAornPayeLockService
{
    Task<bool> UpdateUserAornPayeAttempt(string userRef, bool success);
    Task<UserAornPayeStatus> UserAornPayeStatus(string userRef);
}