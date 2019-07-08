using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Application.Queries;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit;
using SFA.DAS.EAS.Portal.Client.Types;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client
{
    public class PortalClient : IPortalClient
    {
        private readonly GetAccountQuery _getAccountQuery;
        private readonly IDasRecruitService _dasRecruitService;
        
        public PortalClient(IContainer container)
        {
            _getAccountQuery = container.GetInstance<GetAccountQuery>();
            _dasRecruitService = container.GetInstance<IDasRecruitService>();
        }
        
        public async Task<Account> GetAccount(long accountId, bool hasPayeScheme, CancellationToken cancellationToken = default)
        {
            var vacanciesTask = hasPayeScheme ? 
                _dasRecruitService.GetVacancies(accountId, 2, cancellationToken) : null;

            var account = await _getAccountQuery.Get(accountId, cancellationToken);

            if (hasPayeScheme)
            {
                //todo: better to have flag/enum saying none/single/multiple vacancies and have a single instance?
                var vacancies = await vacanciesTask;
                account.Vacancies = vacancies.ToList();
            }

            return account;
        }
    }
}