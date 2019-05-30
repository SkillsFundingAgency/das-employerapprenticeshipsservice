using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.Commands
{
    //todo: rename to ICommand when only 1 interface
    public interface IPortalCommand<TParam>
    {
        Task Execute(TParam param, CancellationToken cancellationToken = default);
    }
}