using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IRefreshEmployerLevyService
    {
        Task QueueRefreshLevyMessage(long accountId, string payeRef);
    }
}
