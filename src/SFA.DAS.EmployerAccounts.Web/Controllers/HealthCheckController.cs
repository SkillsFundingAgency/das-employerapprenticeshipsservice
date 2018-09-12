using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("healthcheck")]
    public class HealthCheckController : Controller
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public HealthCheckController(Lazy<EmployerAccountsDbContext> db)
        {
            _db = db;
        }

        [Route]
        public async Task<ActionResult> Index()
        {
            var healthCheck = await _db.Value.HealthChecks.OrderByDescending(h => h.Id).FirstOrDefaultAsync();

            return View(healthCheck);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route]
        public ActionResult Index(string choice)
        {
            var healthCheck = new HealthCheck();

            healthCheck.PublishEvent();

            _db.Value.HealthChecks.Add(healthCheck);

            return RedirectToAction("Index");
        }
    }
}