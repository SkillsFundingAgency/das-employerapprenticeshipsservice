using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Client.Database.Models
{
    public class DocumentMetadata
    {
        [JsonProperty("schemaVersion")]
        public short SchemaVersion { get; }

        public DocumentMetadata(short schemaVersion)
        {
            SchemaVersion = schemaVersion;
        }
    }
}