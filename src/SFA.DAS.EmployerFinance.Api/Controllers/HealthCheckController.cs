using System.Web.Http;

namespace SFA.DAS.EmployerFinance.Api.Controllers
{
    [RoutePrefix("api/healthcheck")]
    public class HealthCheckController : ApiController
    {
        [Route]
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}