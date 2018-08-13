using SFA.DAS.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    public class BaseController : Controller
    {
        public IAuthenticationService OwinWrapper;
        
        //private readonly IMultiVariantTestingService _multiVariantTestingService;
        //private readonly ICookieStorageService<FlashMessageViewModel> _flashMessage;

        public BaseController(IAuthenticationService owinWrapper/*, IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage*/)
        {
            OwinWrapper = owinWrapper;
            //_multiVariantTestingService = multiVariantTestingService;
            //_flashMessage = flashMessage;
        }
    }
}