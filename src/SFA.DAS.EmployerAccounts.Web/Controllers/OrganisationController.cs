using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
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

        [HttpGet]
        [Route("nextStep")]
        public ActionResult OrganisationAddedNextSteps(
            string organisationName, string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction($"organisations/nextStep?organisationName={organisationName}"));
        }

        [HttpGet]
        [Route("nextStepSearch")]
        public ActionResult OrganisationAddedNextStepsSearch(
            string organisationName, string hashedAccountId, string hashedAgreementId)
        {
            return Redirect(Url.LegacyEasAccountAction($"organisations/nextStepSearch?organisationName={organisationName}&hashedAgreementId={hashedAgreementId}"));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("confirm")]
        public async Task<ActionResult> Confirm(
            string hashedAccountId, string name, string code, string address, DateTime? incorporated,
            string legalEntityStatus, OrganisationType organisationType, byte? publicSectorDataSource, string sector, bool newSearch)
        {
            var request = new CreateNewLegalEntityViewModel
            {
                HashedAccountId = hashedAccountId,
                Name = name,
                Code = code,
                Address = address,
                IncorporatedDate = incorporated,
                ExternalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName),
                LegalEntityStatus = string.IsNullOrWhiteSpace(legalEntityStatus) ? null : legalEntityStatus,
                Source = organisationType,
                PublicSectorDataSource = publicSectorDataSource,
                Sector = sector
            };

            var response = await _orchestrator.CreateLegalEntity(request);

            var flashMessage = new FlashMessageViewModel
            {
                HiddenFlashMessageInformation = "page-organisations-added",
                Headline = $"{response.Data.EmployerAgreement.LegalEntityName} has been added",
                Severity = FlashMessageSeverityLevel.Success
            };
            AddFlashMessageToCookie(flashMessage);
            if (newSearch)
            {
                return RedirectToAction(ControllerConstants.OrganisationAddedNextStepsSearchActionName,
                    new
                    {
                        hashedAccountId,
                        organisationName = name,
                        hashedAgreementId = response.Data.EmployerAgreement.HashedAgreementId
                    });
            }

            return RedirectToAction(ControllerConstants.OrganisationAddedNextStepsActionName,
                new
                {
                    hashedAccountId,
                    organisationName = name,
                    hashedAgreementId = response.Data.EmployerAgreement.HashedAgreementId
                });

        }
    }
}