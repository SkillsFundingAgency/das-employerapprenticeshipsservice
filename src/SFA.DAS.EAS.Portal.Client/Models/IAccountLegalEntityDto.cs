using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Client.Models
{
    //todo: covariant
    public interface IAccountLegalEntityDto<out TReservedFunding> where TReservedFunding : IReservedFundingDto
    {
        long AccountLegalEntityId { get;}
        string LegalEntityName { get; }
        IEnumerable<TReservedFunding> ReservedFundings { get; }
    }
}