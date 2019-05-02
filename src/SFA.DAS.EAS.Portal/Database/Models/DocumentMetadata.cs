using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Database.Models
{
    public class DocumentMetadata
    {
        // chema type by context, i.e. collection?
        // do we want an explicit version anyway?
//        [JsonProperty("schemaType")]
//        public string SchemaType { get; }

        [JsonProperty("schemaVersion")]
        public short SchemaVersion { get; }

//        public DocumentMetadata(string schemaType, short schemaVersion)
//        {
//            SchemaType = schemaType;
//            SchemaVersion = schemaVersion;
//        }

        public DocumentMetadata(short schemaVersion)
        {
            SchemaVersion = schemaVersion;
        }
    }
}