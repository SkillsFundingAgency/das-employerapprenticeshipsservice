using System;
using System.Reflection;
using System.Web;
using System.Web.Http;
using SFA.DAS.Support.Shared;
using SFA.DAS.EAS.Support.Web;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    [Authorize(Roles = "das-support-portal")]
    [System.Web.Mvc.RoutePrefix("api/status")]
    public class StatusController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        // GET: Status
        [System.Web.Mvc.AllowAnonymous]
        public IHttpActionResult Get()
        {
            return Ok(new
            {
                ServiceName = "SFA DAS Employer Apprenticeship Service Support Site",
                ServiceVersion = AddServiceVersion(),
                ServiceTime = DateTimeOffset.UtcNow,
                Request = AddRequestContext()
            });
        }

        private string AddServiceVersion()
        {
            try
            {
                return Assembly.GetExecutingAssembly().Version();
            }
            catch (Exception)
            {
                return "Unknown";
            }
        }
        private string AddRequestContext()
        {
            try
            {
                return $" {HttpContextHelper.Current.Request.HttpMethod}: {HttpContextHelper.Current.Request.RawUrl}";
            }
            catch
            {
                return "Unknown";
            }
        }

    }
}