using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.ReadStore.Mediator
{
    public interface IReadStoreMediator
    {
        Task<TResponse> Send<TResponse>(IReadStoreRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}