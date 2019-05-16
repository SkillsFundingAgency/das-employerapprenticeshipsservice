using Newtonsoft.Json;
using System;

namespace SFA.DAS.EAS.Portal.Database.Models
{
    public interface IDocumentProperties
    {
        [JsonProperty("created")]
        DateTime Created { get; }
        [JsonProperty("updated")]
        DateTime? Updated { get; }
        [JsonProperty("deleted")]
        DateTime? Deleted { get; }
    }
}
