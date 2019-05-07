using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Models;

namespace SFA.DAS.EAS.Portal.Client
{
    public class PortalClient : IPortalClient
    {
        public Task<IAccountDto<IAccountLegalEntityDto<IReservedFundingDto>>> GetAccount(long accountId)
        {
            throw new NotImplementedException();
        }
    }
}