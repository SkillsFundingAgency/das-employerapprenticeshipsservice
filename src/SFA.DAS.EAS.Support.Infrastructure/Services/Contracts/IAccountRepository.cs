using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

public interface IAccountRepository
{
    Task<Core.Models.Account> Get(string hashedAccountId, AccountFieldsSelection selection);
    Task<IEnumerable<Core.Models.Account>> FindAllDetails(int pageSize, int pageNumber);
    Task<int> TotalAccountRecords(int pagesize);
}