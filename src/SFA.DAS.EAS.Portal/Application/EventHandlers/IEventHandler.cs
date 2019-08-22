using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.EventHandlers
{
    public interface IEventHandler<TEvent>
    {
        Task Handle(TEvent @event, CancellationToken cancellationToken = default);
    }
}