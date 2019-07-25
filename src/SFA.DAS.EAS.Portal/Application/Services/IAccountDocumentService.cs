using SFA.DAS.EAS.Portal.Client.Database.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.Services
{
    public interface IAccountDocumentService
    {
        Task<AccountDocument> Get(long id, CancellationToken cancellationToken = default);
        Task<AccountDocument> GetOrCreate(long id, CancellationToken cancellationToken = default);
        Task Save(AccountDocument account, CancellationToken cancellationToken = default);
    }
}
