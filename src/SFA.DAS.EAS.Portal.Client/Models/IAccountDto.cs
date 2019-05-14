using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Client.Models
{
    //todo: can we simplify as we're using covariant?
    public interface IAccountDto
    {
        long AccountId { get; }
        IEnumerable<IAccountLegalEntityDto> AccountLegalEntities { get; }
        DateTime? Deleted { get; }
    }
}