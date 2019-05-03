using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.TempEvents;

namespace SFA.DAS.EAS.Portal.Application.Commands
{
    public interface IAddReserveFundingCommand
    {
        Task Execute(ReserveFundingAddedEvent reservedFunding, string messageId,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}