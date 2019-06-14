
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.EventHandlers
{
    public interface IEventHandler<T>
    {   
        Task Handle(T @event, CancellationToken cancellationToken = default);
    }
}
