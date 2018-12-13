using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IJobHistoryRepository
    {
        Task<bool> HasJobRun(string job);
        Task MarkJobAsRan(string job);
    }
}