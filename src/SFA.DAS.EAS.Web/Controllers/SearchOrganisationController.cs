using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/organisations/search")]
    public class SearchOrganisationController : BaseController
    {
        private readonly SearchOrganisationOrchestrator _orchestrator;
        

        public SearchOrganisationController(IOwinWrapper owinWrapper,
            SearchOrganisationOrchestrator orchestrator,
            IFeatureToggle featureToggle,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("search")]
        public ActionResult SearchForOrganisation()
        {
            return View("SearchForOrganisation");
        }

        [HttpPost]
        [Route("results")]
        public async Task<ActionResult> SearchForOrganisationResults(string searchTerm)
        {
            var model = await _orchestrator.SearchOrganisation(searchTerm);

            return View("SearchForOrganisationResults", model);
        }


    }
}