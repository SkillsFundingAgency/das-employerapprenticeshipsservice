using System.Web.Http;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [RoutePrefix("ping")]
    public class PingController : ApiController
    {
        [Route]
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}