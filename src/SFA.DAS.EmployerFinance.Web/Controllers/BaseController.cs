using SFA.DAS.Authentication;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    public class BaseController : Controller
    {
        public IAuthenticationService OwinWrapper;
        
        public BaseController(IAuthenticationService owinWrapper)
        {
            OwinWrapper = owinWrapper;
        }
    }
}