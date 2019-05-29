using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ProviderRelationships.Messages.Events;

namespace SFA.DAS.EAS.Portal.Application.Commands.ProviderPermissions
{
    public interface IAddAccountProviderCommand
    {
        Task Execute(AddedAccountProviderEvent addedAccountProviderEvent,
            CancellationToken cancellationToken = default);
    }
}