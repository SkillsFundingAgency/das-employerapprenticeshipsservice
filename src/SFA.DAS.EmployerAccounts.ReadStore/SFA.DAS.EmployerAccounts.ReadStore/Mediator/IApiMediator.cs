using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.ReadStore.Mediator
{
    public interface IApiMediator
    {
        Task<TResponse> Send<TResponse>(IApiRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}