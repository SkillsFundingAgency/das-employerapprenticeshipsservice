using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Client.Models
{
    public interface IAccountLegalEntityDto
    {
        long AccountLegalEntityId { get;}
        string LegalEntityName { get; }
        IEnumerable<IReservedFundingDto> ReservedFundings { get; }
    }
}