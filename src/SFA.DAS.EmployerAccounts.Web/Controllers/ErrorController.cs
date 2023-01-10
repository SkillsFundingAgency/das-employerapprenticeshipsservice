namespace SFA.DAS.EmployerAccounts.Web.Controllers;

public class ErrorController : Controller
{
    [Route("accessdenied")]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return View();
    }

    [Route("error")]
    public IActionResult Error()
    {
        Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return View();
    }

    [Route("notfound")]
    public IActionResult NotFound()
    {
        Response.StatusCode = (int)HttpStatusCode.NotFound;

        return View();
    }
}