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

        public RedirectController(EmployerApprenticeshipsServiceConfiguration cofiguration)
        {
            _configuration = cofiguration;
        }

        protected IHttpActionResult RedirectToEmployerAccountsApi(string requestUriPathAndQuery)
        {
            return Redirect(Url.Action(_configuration.EmployerAccountsApiBaseUrl, Request.RequestUri.PathAndQuery));
        }
    }
}