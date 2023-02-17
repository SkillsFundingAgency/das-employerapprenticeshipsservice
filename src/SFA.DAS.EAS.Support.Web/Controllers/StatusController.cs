using System;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Support.Shared;
using SFA.DAS.EAS.Support.Web;
using Microsoft.AspNetCore.Http.Extensions;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    [Authorize(Roles = "das-support-portal")]
    [Route("api/status")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        // GET: Status
        [System.Web.Mvc.AllowAnonymous]
        public IActionResult Get()
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
                return $" {HttpContext.Request.Method}: {HttpContext.Request.GetDisplayUrl()}";
            }
            catch
            {
                return "Unknown";
            }
        }

    }
}