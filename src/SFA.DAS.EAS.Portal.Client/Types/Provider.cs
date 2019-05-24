using System.Collections.Generic;
using Newtonsoft.Json;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.EAS.Portal.Client.Types
{
    public class Provider
    {
        [JsonProperty("ukprn")]
        public long Ukprn { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        //todo: will add entourage of SFA.DAS.ProviderRelationships.Types to consumer. ok?
        public ICollection<Operation> GrantedOperations { get; set; }

    }
}