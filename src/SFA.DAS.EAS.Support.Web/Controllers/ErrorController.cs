using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[ExcludeFromCodeCoverage]
public class ErrorController : Controller
{
    public IActionResult Error()
    {
        return View();
    }


    public IActionResult NotFound()
    {
        return View("Error");
    }

    public IActionResult BadRequest()
    {
        return View("Error");
    }
}