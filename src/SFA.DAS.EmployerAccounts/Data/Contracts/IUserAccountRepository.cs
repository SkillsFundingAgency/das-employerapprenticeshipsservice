using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Data.Contracts;

public interface IUserAccountRepository
{
    Task<Accounts<Account>> GetAccountsByUserRef(string userRef);
    Task<User> Get(string email);
    Task<User> Get(long id);
    Task<User> GetUserByRef(Guid @ref);
    Task Upsert(User user);
    Task<Accounts<Account>> GetAccounts();
}