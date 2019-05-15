using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Types;

namespace SFA.DAS.EAS.Portal.Client.Models.Concrete
{
    public class AccountDto : IAccountDto
    {
        public AccountDto(IEnumerable<AccountLegalEntityDto> accountLegalEntities)
        {
            AccountLegalEntities = accountLegalEntities;
        }

        [JsonProperty("accountId")]
        public virtual long AccountId { get; private set; }

        //todo: return List in dto's, not enumerable (so consumer can add to them without copying - return most specific type)
        // https://stackoverflow.com/questions/15490633/why-cant-i-use-a-compatible-concrete-type-when-implementing-an-interface
        [JsonProperty("accountLegalEntities")]
        public virtual IEnumerable<IAccountLegalEntityDto> AccountLegalEntities { get; private set; }

        [JsonProperty("organisations")]
        public virtual IEnumerable<Organisation> Organisations { get; private set; }

        [JsonProperty("deleted")]
        public virtual DateTime? Deleted { get; private set; }
    }
}