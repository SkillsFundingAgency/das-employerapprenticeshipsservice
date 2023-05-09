using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

public interface IAccountRepository
{
    Task<Core.Models.Account> Get(string id, AccountFieldsSelection selection);
    Task<decimal> GetAccountBalance(string id);
    Task<IEnumerable<Core.Models.Account>> FindAllDetails(int pagesize, int pageNumber);
    Task<int> TotalAccountRecords(int pagesize);
}