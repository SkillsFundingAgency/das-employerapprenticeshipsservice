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
        private readonly IGetAccountQuery _getAccountQuery;
        private readonly IDasRecruitService _dasRecruitService;
        
        public PortalClient(IContainer container)
        {
            _getAccountQuery = container.GetInstance<IGetAccountQuery>();
            _dasRecruitService = container.GetInstance<IDasRecruitService>();
        }
        
        //todo: might be better to just accept publicHashedAccountId, and decode it here
        public async Task<Account> GetAccount(long accountId, string publicHashedAccountId,
            AccountState accountState, CancellationToken cancellationToken = default)
        {
            var hasPayeScheme = (accountState & AccountState.HasPayeScheme) == AccountState.HasPayeScheme;

            // we potentially map 1 more vacancy than necessary, but it keeps the code clean
            var vacanciesTask = hasPayeScheme ? 
                _dasRecruitService.GetVacancies(publicHashedAccountId, 2, cancellationToken) : null;

            var account = await _getAccountQuery.Get(accountId, cancellationToken);

            // at a later date, we might want to create an empty account doc and add the vacancy details to it, but for now, let's keep it simple
            if (!hasPayeScheme || account == null)
                return account;
            
            var vacancies = await vacanciesTask;
            if (vacancies == null)
                return account;
            
            var vacanciesCount = vacancies.Count();
            switch (vacanciesCount)
            {
                case 0:
                    account.VacancyCardinality = Cardinality.None;
                    break;
                case 1:
                    account.VacancyCardinality = Cardinality.One;
                    account.SingleVacancy = vacancies.Single();
                    break;
                default:
                    account.VacancyCardinality = Cardinality.Many;
                    break;
            }

            return account;
        }
    }
}