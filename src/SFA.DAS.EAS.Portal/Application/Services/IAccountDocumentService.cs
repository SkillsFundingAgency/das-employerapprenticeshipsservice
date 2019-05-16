using SFA.DAS.EAS.Portal.Database.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.Services
{
    public interface IAccountDocumentService
    {
        Task<AccountDocument> Get(long id, CancellationToken cancellationToken = new CancellationToken());
        Task Save(AccountDocument account, CancellationToken cancellationToken = new CancellationToken());
    }
}
