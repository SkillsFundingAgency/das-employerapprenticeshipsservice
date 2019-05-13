using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Client.Models
{
    public interface IAccountLegalEntityDto<out TReservedFunding> where TReservedFunding : IReservedFundingDto
    {
        long AccountLegalEntityId { get;}
        string LegalEntityName { get; }
        IEnumerable<TReservedFunding> ReservedFundings { get; }
    }
}