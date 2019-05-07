using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Client.Models.Concrete
{
    internal class AccountDto : IAccountDto<IAccountLegalEntityDto<IReservedFundingDto>>
    {
        [JsonProperty("accountId")]
        public long AccountId { get; private set; }

        [JsonProperty("accountLegalEntities")]
        public IEnumerable<IAccountLegalEntityDto<IReservedFundingDto>> AccountLegalEntities { get; private set; }

        [JsonProperty("deleted")]
        public virtual DateTime? Deleted { get; private set; }
    }
}