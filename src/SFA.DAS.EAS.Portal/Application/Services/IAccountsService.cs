using SFA.DAS.EAS.Portal.Types;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.Services
{
    public interface IAccountsService
    {
        Task<Account> Get(string id);
        Task Save(Account account);
    }
}
