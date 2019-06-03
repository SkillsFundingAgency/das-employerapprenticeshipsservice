using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SFA.DAS.CosmosDb;

namespace SFA.DAS.EAS.Portal.Client.Database.Models
{
    public abstract class Document : IDocument, IDocumentProperties, IOutbox
    {
        [JsonProperty("id")]
        public Guid Id { get; protected set; }

        [JsonIgnore]
        public bool IsNew { get; set; }

        [JsonIgnore]
        public string ETag { get; protected set; }

        [JsonProperty("_etag")]
        private string ReadOnlyETag { set => ETag = value; }

        [JsonProperty("metadata")]
        public DocumentMetadata Metadata { get; protected set; }

        [JsonProperty("created")]
        public DateTime Created { get;  set; }

        [JsonProperty("updated")]
        public DateTime? Updated { get; set; }

        [JsonProperty("deleted")]
        public DateTime? Deleted { get; set; }

        protected Document(short schemaVersion)
        {
            Metadata = new DocumentMetadata(schemaVersion);
        }

        [JsonProperty("outboxData")]
        public IEnumerable<OutboxMessage> OutboxData => _outboxData;

        [JsonIgnore]
        private List<OutboxMessage> _outboxData = new List<OutboxMessage>();

        public void DeleteOldMessages()
        {
            //todo: configurable. think what we want to set this to
            var expiryPeriod = TimeSpan.Parse("2");
            //todo: unit testable
            var now = DateTime.UtcNow;
            _outboxData = _outboxData.Where(m => now - m.Created < expiryPeriod).ToList();
        }

        public void AddOutboxMessage(string messageId, DateTime created)
        {
            if (messageId == null)
                throw new ArgumentNullException(nameof(messageId));

            _outboxData.Add(new OutboxMessage(messageId, created));
        }

        public bool IsMessageProcessed(string messageId)
        {
            return OutboxData.Any(m => m.MessageId == messageId);
        }

        protected Document()
        {
            
        }
    }
}