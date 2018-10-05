using MediatR;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public abstract class EmployerVerificationOrchestratorBase
    {
        protected readonly IMediator Mediator;
        protected readonly ILog Logger;
        protected readonly ICookieStorageService<EmployerAccountData> CookieService;
        protected readonly EmployerAccountsConfiguration Configuration;

        //Needed for tests
        protected EmployerVerificationOrchestratorBase()
        {

        }

        protected EmployerVerificationOrchestratorBase(IMediator mediator, ILog logger, ICookieStorageService<EmployerAccountData> cookieService, EmployerAccountsConfiguration configuration)
        {
            Mediator = mediator;
            Logger = logger;
            CookieService = cookieService;
            Configuration = configuration;
        }


        public virtual async Task<GetUserAccountRoleResponse> GetUserAccountRole(string hashedAccountId, string externalUserId)
        {
            return await Mediator.SendAsync(new GetUserAccountRoleQuery
            {
                HashedAccountId = hashedAccountId,
                ExternalUserId = externalUserId
            });
        }
    }
}