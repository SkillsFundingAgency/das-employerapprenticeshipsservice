using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IRefreshEmployerLevyService
    {
        Task QueueRefreshLevyMessage(long accountId, string payeRef);
    }
}
