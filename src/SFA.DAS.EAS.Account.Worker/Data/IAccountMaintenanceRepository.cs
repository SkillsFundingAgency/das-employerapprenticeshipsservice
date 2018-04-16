using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Account.Worker.Data
{
    public interface IAccountMaintenanceRepository
    {
        Task<List<long>> GetAccountsMissingPublicHashedId();
        Task UpdateAccountPublicHashedIds(IEnumerable<KeyValuePair<long, string>> accounts);
    }
}