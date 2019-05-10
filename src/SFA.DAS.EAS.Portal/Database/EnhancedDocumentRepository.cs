using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.EAS.Portal.Client.Data;
using SFA.DAS.EAS.Portal.Database.Models;

namespace SFA.DAS.EAS.Portal.Database
{
    public class EnhancedDocumentRepository<TDocument, TId> : EnhancedReadOnlyDocumentRepository<TDocument, TId>, IEnhancedDocumentRepository<TDocument, TId> where TDocument : class, IDocument
    {
        public EnhancedDocumentRepository(IDocumentClient documentClient, string databaseName, string collectionName)
            : base(documentClient, databaseName, collectionName)
        {
        }

        public virtual Task Add(TDocument document, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            
//            if (EqualityComparer<TId>.Default.Equals(document.Id, default))
//                throw new Exception("Id must not be default");
            if (document.Id == null)
                throw new Exception("Document Id must be supplied");
            
            return DocumentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), document, requestOptions, true, cancellationToken);
        }
        
        public virtual Task Remove(TId id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            return DocumentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, id.ToString()), requestOptions, cancellationToken);
        }

        public virtual Task Update(TDocument document, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            
//            if (EqualityComparer<TId>.Default.Equals(document.Id, default))
//                throw new Exception("Id must not be default");
            if (document.Id == null)
                throw new Exception("Document Id must be supplied");

            requestOptions = AddOptimisticLockingIfETagSetAndNoAccessConditionDefined(document, requestOptions);

            return DocumentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, document.Id.ToString()), document, requestOptions, cancellationToken);
        }

        private RequestOptions AddOptimisticLockingIfETagSetAndNoAccessConditionDefined(IDocument document, RequestOptions requestOptions)
        {
            var options = requestOptions ?? new RequestOptions();
            if (options.AccessCondition == null)
            {
                if (!string.IsNullOrWhiteSpace(document.ETag))
                {
                    options.AccessCondition = new AccessCondition {
                        Condition = document.ETag,
                        Type = AccessConditionType.IfMatch
                    };
                }
            }
            return options;
        }
    }
}