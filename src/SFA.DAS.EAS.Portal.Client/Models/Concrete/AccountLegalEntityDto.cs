using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Client.Models.Concrete
{
    public class AccountLegalEntityDto : IAccountLegalEntityDto<IReservedFundingDto>
    {
        public AccountLegalEntityDto(IEnumerable<ReservedFundingDto> reservedFundings)
        {
            ReservedFundings = reservedFundings;
        }

        [JsonProperty("accountLegalEntityId")]
        public virtual long AccountLegalEntityId { get; private set; }

        [JsonProperty("legalEntityName")]
        public virtual string LegalEntityName { get; private set; }
        
        [JsonProperty("reservedFundings")]
        public virtual IEnumerable<IReservedFundingDto> ReservedFundings { get; private set; }
    }
}