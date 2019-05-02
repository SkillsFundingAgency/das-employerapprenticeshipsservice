using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Database.Models
{
    public class AccountLegalEntity
    {
        //todo: rename to id & name?
        [JsonProperty("accountLegalEntityId")]
        public long AccountLegalEntityId { get; private set; }

        [JsonProperty("legalEntityName")]
        public long LegalEntityName { get; private set; }
        
        [JsonProperty("reservedFundings")]
        public IEnumerable<ReservedFunding> ReservedFundings => _reservedFundings;

        [JsonIgnore]
        private readonly List<ReservedFunding> _reservedFundings = new List<ReservedFunding>();
    }
}