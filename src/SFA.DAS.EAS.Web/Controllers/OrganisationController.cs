using AutoMapper;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}/organisations")]
    public class OrganisationController : BaseController
    {
        private readonly OrganisationOrchestrator _orchestrator;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public OrganisationController(
            IAuthenticationService owinWrapper,
            OrganisationOrchestrator orchestrator,
            IAuthorizationService authorization,
            IMultiVariantTestingService multiVariantTestingService,
            IMapper mapper,
            ILog logger,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("legalAgreement")]
        public ActionResult OrganisationLegalAgreement(string hashedAccountId, OrganisationDetailsViewModel model)
        {
            var viewModel = new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = model,
                Status = HttpStatusCode.OK
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("confirm")]
        public async Task<ActionResult> Confirm(
            string hashedAccountId, string name, string code, string address, DateTime? incorporated,
            string legalEntityStatus, OrganisationType organisationType, byte? publicSectorDataSource, string sector, bool newSearch)
        {
            return Redirect(Url.EmployerAccountsAction("organisations/confirm"));
        }

        [HttpGet]
        [Route("nextStep")]
        public async Task<ActionResult> OrganisationAddedNextSteps(string organisationName, string hashedAccountId, string hashedAgreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"organisations/nextStep"));
        }
        
        [HttpGet]
        [Route("nextStepSearch")]
        public async Task<ActionResult> OrganisationAddedNextStepsSearch(string organisationName, string hashedAccountId, string hashedAgreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"organisations/nextStepSearch"));
        }


        [HttpPost]
        [Route("nextStep")]
        public async Task<ActionResult> GoToNextStep(string nextStep, string hashedAccountId, string organisationName, string hashedAgreementId)
        {
            return Redirect(Url.EmployerAccountsAction("nextStep"));
        }

        [HttpGet]
        [Route("review")]
        public async Task<ActionResult> Review(string hashedAccountId, string accountLegalEntityPublicHashedId)
        {
            return Redirect(Url.EmployerAccountsAction("organisations/review"));
        }

        [HttpPost]
        [Route("review")]
        public async Task<ActionResult> ProcessReviewSelection(
            string updateChoice,
            string hashedAccountId,
            string accountLegalEntityPublicHashedId,
            string organisationName,
            string organisationAddress,
            string dataSourceFriendlyName)
        {
            return Redirect(Url.EmployerAccountsAction("organisations/review"));
        }

        [HttpPost]
        [Route("PostUpdateSelection")]
        public ActionResult GoToPostUpdateSelection(string nextStep, string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("organisations/PostUpdateSelection"));
        }
    }
}