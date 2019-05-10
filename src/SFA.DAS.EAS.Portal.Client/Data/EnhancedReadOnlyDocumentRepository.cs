using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.CosmosDb;

namespace SFA.DAS.EAS.Portal.Client.Data
{
    //todo: update ReadOnlyDocumentRepository directly in shared (hence current rubbish name)
    public class EnhancedReadOnlyDocumentRepository<TDocument, TId> 
        : ReadOnlyDocumentRepository<TDocument>, IEnhancedReadOnlyDocumentRepository<TDocument, TId> where TDocument : class
    {
        public EnhancedReadOnlyDocumentRepository(IDocumentClient documentClient, string databaseName, string collectionName)
            : base(documentClient, databaseName, collectionName)
        {
        }
        
        public virtual async Task<TDocument> GetById(TId id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await DocumentClient.ReadDocumentAsync<TDocument>(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, id.ToString()), requestOptions, cancellationToken).ConfigureAwait(false);

                return response.Document;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    return default;

                throw;
            }
        }
    }
}