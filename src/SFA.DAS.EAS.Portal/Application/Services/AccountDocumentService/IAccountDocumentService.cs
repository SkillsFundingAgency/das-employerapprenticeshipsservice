using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Database.Models;

namespace SFA.DAS.EAS.Portal.Application.Services.AccountDocumentService
{
    public interface IAccountDocumentService
    {
        Task<AccountDocument> Get(long id, CancellationToken cancellationToken = default);
        Task<AccountDocument> GetOrCreate(long id, CancellationToken cancellationToken = default);
        Task Save(AccountDocument account, CancellationToken cancellationToken = default);
    }
}
