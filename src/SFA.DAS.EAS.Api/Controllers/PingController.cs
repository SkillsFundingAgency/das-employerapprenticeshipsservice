using System.Web.Http;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/ping")]
    public class PingController : ApiController
    {
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}