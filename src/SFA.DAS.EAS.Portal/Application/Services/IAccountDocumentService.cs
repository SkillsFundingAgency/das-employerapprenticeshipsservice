using SFA.DAS.EAS.Portal.Client.Database.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.Services
{
    public interface IAccountDocumentServiceReadOnly
    {
        Task<AccountDocument> GetOrCreate(long id, CancellationToken cancellationToken = default);
        Task<AccountDocument> Get(long id, CancellationToken cancellationToken = default);
    }

    public interface IAccountDocumentService : IAccountDocumentServiceReadOnly
    {
        Task Save(AccountDocument account, CancellationToken cancellationToken = default);
    }
}
