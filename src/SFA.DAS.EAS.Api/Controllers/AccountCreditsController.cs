using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SFA.DAS.EAS.Api.Controllers
{
   [RoutePrefix("api/credits")]
    public class AccountCreditsController : ApiController
    {
        [Route("")]
       public IHttpActionResult Get()
       {
           return Ok();
       }
    }
}
