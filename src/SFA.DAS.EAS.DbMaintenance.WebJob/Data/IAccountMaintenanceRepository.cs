using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Data
{
    public interface IAccountMaintenanceRepository
    {
        Task<List<long>> GetAccountsMissingPublicHashedId();
        Task UpdateAccountPublicHashedIds(IEnumerable<KeyValuePair<long, string>> accounts);
    }
}