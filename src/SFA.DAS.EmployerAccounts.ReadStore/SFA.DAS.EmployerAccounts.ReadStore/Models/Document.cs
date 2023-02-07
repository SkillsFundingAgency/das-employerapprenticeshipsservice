using System;
using Newtonsoft.Json;
using SFA.DAS.CosmosDb;

namespace SFA.DAS.EmployerAccounts.ReadStore.Models;

public abstract class Document : IDocument
{
    [JsonProperty("id")]
    public Guid Id { get; protected set; }

    [JsonIgnore]
    public string ETag { get; private set; }

    [JsonProperty("_etag")]
    private string ReadOnlyETag { set => ETag = value; }

    [JsonProperty("metadata")]
    public DocumentMetadata Metadata { get; protected set;}

    protected Document(short schemaVersion, string schemaType)
    {
        Metadata = new DocumentMetadata(schemaVersion, schemaType);
    }

    protected Document()
    {
    }
}