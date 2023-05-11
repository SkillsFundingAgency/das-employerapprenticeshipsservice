namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserById(long id);
    Task<User> GetUserByRef(Guid @ref);
    Task<User> GetUserByRef(string id);
    Task<User> GetByEmailAddress(string emailAddress);
    Task Create(User user);
    Task Update(User user);
    Task Upsert(User user);
    Task<Users> GetAllUsers();
    Task UpdateAornPayeQueryAttempt(string userRef, bool success);
    Task<IEnumerable<DateTime>> GetAornPayeQueryAttempts(string userRef);
    Task UpdateTermAndConditionsAcceptedOn(string userRef);
}