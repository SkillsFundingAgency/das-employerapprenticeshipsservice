using System.Web;
using MediatR;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class OrganisationOrchestrator : UserVerificationOrchestratorBase, IOrchestratorCookie
    {
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";
        private readonly ICookieStorageService<EmployerAccountData> _cookieService;

        public OrganisationOrchestrator(
            IMediator mediator,
            ICookieStorageService<EmployerAccountData> cookieService
        )
            : base(mediator)
        {
            _cookieService = cookieService;
        }


        public virtual EmployerAccountData GetCookieData(HttpContextBase context)
        {
            return _cookieService.Get(CookieName);
        }

        public virtual void CreateCookieData(HttpContextBase context, EmployerAccountData data)
        {
            _cookieService.Create(data, CookieName, 365);
        }
    }
}