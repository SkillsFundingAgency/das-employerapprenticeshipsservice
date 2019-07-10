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
            bool hasPayeScheme, CancellationToken cancellationToken = default)
        {
            // we potentially map 1 more vacancy tha necessary, but it keeps the code clean
            var vacanciesTask = hasPayeScheme ? 
                _dasRecruitService.GetVacancies(publicHashedAccountId, 2, cancellationToken) : null;

            var account = await _getAccountQuery.Get(accountId, cancellationToken);

            if (!hasPayeScheme)
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