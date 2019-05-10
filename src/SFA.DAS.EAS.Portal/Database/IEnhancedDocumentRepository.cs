using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace SFA.DAS.EAS.Portal.Database
{
    public interface IEnhancedDocumentRepository<TDocument, TId> //: IDocumentRepository<TDocument> where TDocument : class //, IDocument<TId>
    {
        Task Add(TDocument document, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
        IQueryable<TDocument> CreateQuery(FeedOptions feedOptions = null);
        Task<TDocument> GetById(TId id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
        Task Remove(TId id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
        Task Update(TDocument document, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    }
}