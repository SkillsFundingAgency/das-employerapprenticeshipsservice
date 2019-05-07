using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Client.Models
{
    //todo: can we simplify as we're using covariant?
    public interface IAccountDto<out TAccountLegalEntityDto> where TAccountLegalEntityDto : IAccountLegalEntityDto<IReservedFundingDto>
    {
        IEnumerable<TAccountLegalEntityDto> AccountLegalEntities { get; }

        //needed?
        //DateTime? Deleted { get; }
    }
}