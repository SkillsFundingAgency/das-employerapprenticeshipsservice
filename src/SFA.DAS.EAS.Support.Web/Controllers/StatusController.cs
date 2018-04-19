using System;
using System.Reflection;
using System.Web;
using System.Web.Http;
using SFA.DAS.Support.Shared;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    [System.Web.Mvc.RoutePrefix("api/status")]
    public class StatusController : ApiController
    {
        // GET: Status
        [System.Web.Mvc.AllowAnonymous]
        public IHttpActionResult Get()
        {
            return Ok(new
            {
                ServiceName = AddServiceName(),
                ServiceVersion = AddServiceVersion(),
                ServiceTime = AddServerTime(),
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
                return $" {HttpContext.Current.Request.HttpMethod}: {HttpContext.Current.Request.RawUrl}";
            }
            catch
            {
                return "Unknown";
            }
        }

        private DateTimeOffset AddServerTime()
        {
            return DateTimeOffset.UtcNow;
        }

        private string AddServiceName()
        {
            try
            {
                return "SFA DAS Employer Apprenticeship Service Support Site";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}