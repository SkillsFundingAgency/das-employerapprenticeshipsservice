using System.Threading.Tasks;

namespace SFA.DAS.EAS.TestCommon.DbCleanup
{
    public interface ICleanTransactionsDatabase
    {
        Task Execute();
    }
}