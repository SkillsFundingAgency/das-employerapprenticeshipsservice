using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.ReadStore.Models;

public class DocumentMetadata
{
    [JsonProperty("schemaType")]
    public string SchemaType { get; set; }

    [JsonProperty("schemaVersion")]
    public short SchemaVersion { get; set; }

    public DocumentMetadata(short schemaVersion, string schemaType)
    {
        SchemaType = schemaType;
        SchemaVersion = schemaVersion;
    }
}