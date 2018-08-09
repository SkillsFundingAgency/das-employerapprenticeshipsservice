using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("tasks/{hashedAccountId}")]
    public class TaskController : Controller
    {

    }
}