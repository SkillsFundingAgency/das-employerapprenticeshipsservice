using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Models;

namespace SFA.DAS.EAS.Portal.Client
{
    public interface IPortalClient
    {
        Task<IAccountDto<IAccountLegalEntityDto<IReservedFundingDto>>> GetAccount(long accountId);
    }
}