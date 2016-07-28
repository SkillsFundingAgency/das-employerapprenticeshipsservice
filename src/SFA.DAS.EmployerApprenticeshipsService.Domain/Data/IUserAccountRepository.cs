using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IUserAccountRepository : IRepository
    {
        Task<Accounts> GetAccountsByUserId(string userId);
        Task<User> Get(string email);
        Task<User> Get(long id);
    }
}
