using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Database.Models
{
    public abstract class Document : IDocument
    {
        [JsonProperty("id")]
        public string Id { get; protected set; }

        [JsonIgnore]
        public string ETag { get; protected set; }

        [JsonProperty("_etag")]
        private string ReadOnlyETag { set => ETag = value; }

        [JsonProperty("metadata")]
        public DocumentMetadata Metadata { get; protected set;}

        protected Document(short schemaVersion)
        {
            Metadata = new DocumentMetadata(schemaVersion);
        }
        
        protected Document()
        {
        }
    }
}