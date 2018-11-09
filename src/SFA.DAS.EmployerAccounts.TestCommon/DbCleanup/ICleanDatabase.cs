using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.TestCommon.DbCleanup
{
    public interface ICleanDatabase
    {
        Task Execute();
    }
}