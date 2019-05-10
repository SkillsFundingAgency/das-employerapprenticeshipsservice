using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.CosmosDb;

namespace SFA.DAS.EAS.Portal.Client.Data
{
    public interface IEnhancedReadOnlyDocumentRepository<TDocument, TId> : IReadOnlyDocumentRepository<TDocument> where TDocument : class
    {
        Task<TDocument> GetById(TId id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    }
}