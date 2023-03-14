using SFA.DAS.EAS.Web.Extensions;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Configuration;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Authorization.Mvc.Attributes;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{HashedAccountId}/transfers")]
    public class TransfersController : Microsoft.AspNetCore.Mvc.Controller
    {
        public IConfiguration Configuration { get; set; }
        public TransfersController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        [Route]
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return Redirect(Url.EmployerFinanceAction("transfers/connections", Configuration));
        }
    }
}