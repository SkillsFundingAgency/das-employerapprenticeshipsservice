using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/organisations")]
    public class EmployerAccountOrganisationController : BaseController
    {
        private readonly OrganisationOrchestrator _orchestrator;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public EmployerAccountOrganisationController(
            IOwinWrapper owinWrapper,
            OrganisationOrchestrator orchestrator,
            IFeatureToggle featureToggle,
            IMultiVariantTestingService multiVariantTestingService,
            IMapper mapper,
            ILog logger,
            ICookieStorageService<FlashMessageViewModel> flashMessage) 
            :base(owinWrapper, featureToggle,multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
            _mapper = mapper;
            _logger = logger;
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("address/update")]
        public ActionResult UpdateOrganisationAddress(AddOrganisationAddressViewModel request)
        {
            var response = _orchestrator.AddOrganisationAddress(request);

            if (response.Status == HttpStatusCode.BadRequest)
            {
                request.Address = request.Address ?? new AddressViewModel();
                request.Address.ErrorDictionary = response.Data.ErrorDictionary;

                var errorResponse = new OrchestratorResponse<AddOrganisationAddressViewModel>
                {
                    Data = request,
                    Status = HttpStatusCode.BadRequest,
                    Exception = response.Exception,
                    FlashMessage = response.FlashMessage
                };

                return View(ControllerConstants.AddOrganisationAddressViewName, errorResponse);
            }
            response.Data.CreateOrganisationCookie(_orchestrator, HttpContext);

            return RedirectToAction(ControllerConstants.GatewayInformViewName, ControllerConstants.EmployerAccountViewName, response.Data);
        }
        
        
    }
}