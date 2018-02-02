using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class TransferRepository : BaseRepository, ITransferRepository
    {
        public TransferRepository(string connectionString, ILog logger) : base(connectionString, logger)
        {
        }

        public Task<double> GetTransferBalance(string hashedAccountId)
        {
            throw new System.NotImplementedException();
        }
    }
}
