using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class EmployerCommitmentsController : BaseController
    {
        private readonly IOwinWrapper _owinWrapper;
        private readonly EmployerCommitmentsOrchestrator _employerCommitmentsOrchestrator;

        public EmployerCommitmentsController(IOwinWrapper owinWrapper,
            EmployerCommitmentsOrchestrator employerCommitmentsOrchestrator)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            if (employerCommitmentsOrchestrator == null)
                throw new ArgumentNullException(nameof(employerCommitmentsOrchestrator));
            _owinWrapper = owinWrapper;
            _employerCommitmentsOrchestrator = employerCommitmentsOrchestrator;
        }

        [HttpGet]
        public async Task<ActionResult> Index(long accountid)
        {
            var model = await _employerCommitmentsOrchestrator.GetAll(accountid);

            return View(model);
        }
    }
}