using System.Web.Http;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [RoutePrefix("ping")]
    public class PingController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        [Route]
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}