using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.TestCommon.DbCleanup
{
    public interface ICleanTransactionsDatabase
    {
        Task Execute();
    }
}