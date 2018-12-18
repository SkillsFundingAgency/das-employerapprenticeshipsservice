using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Extensions;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    public class RedirectController : ApiController
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        private RedirectController()
        {

        }

        public RedirectController(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected IHttpActionResult RedirectToEmployerAccountsApi()
        {
            return Redirect(Url.Action(_configuration.EmployerAccountsApiBaseUrl, Request.RequestUri.PathAndQuery));
        }

        protected IHttpActionResult RedirectToEmployerFinanceApi()
        {
            return Redirect(Url.Action(_configuration.EmployerFinanceApiBaseUrl, Request.RequestUri.PathAndQuery));
        }

    }
}