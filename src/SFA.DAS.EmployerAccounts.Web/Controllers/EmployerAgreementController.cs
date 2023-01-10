using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesCountByHashedAccountId;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetProviderInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Route("accounts/{HashedAccountId}")]
    public class EmployerAgreementController : BaseController
    {
        private const int InvitationComplete = 4;
        private readonly EmployerAgreementOrchestrator _orchestrator;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IUrlActionHelper _urlActionHelper;
        private const int ViewAgreementNow = 1;
        private const int ViewAgreementLater = 2;

        public EmployerAgreementController(IAuthenticationService owinWrapper,
            EmployerAgreementOrchestrator orchestrator,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            IMediator mediator,
            IMapper mapper,
            IUrlActionHelper urlActionHelper)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
            _mediator = mediator;
            _mapper = mapper;
            _urlActionHelper = urlActionHelper;
        }

        public EmployerAgreementController(IAuthenticationService owinWrapper) : base(owinWrapper)
        {
        }

        [HttpGet]
        [DasAuthorize(EmployerUserRole.Any)]
        [Route("agreements")]
        public async Task<IActionResult> Index(string hashedAccountId, bool agreementSigned = false)
        {
            var model = await _orchestrator.Get(hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            var flashMessage = GetFlashMessageViewModelFromCookie();
            if (flashMessage != null)
            {
                model.FlashMessage = flashMessage;
            }

            ViewBag.ShowConfirmation = agreementSigned && model.Data.EmployerAgreementsData.HasPendingAgreements;

            return View(model);
        }

        [HttpGet]
        [DasAuthorize(EmployerUserRole.Any)]
        [Route("agreements/{agreementId}/details")]
        public async Task<IActionResult> Details(string agreementId, string hashedAccountId,
            FlashMessageViewModel flashMessage)
        {
            var agreement = await _orchestrator.GetById(
                agreementId,
                hashedAccountId,
                OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName)
            );

            return View(agreement);
        }

        [HttpGet]
        [DasAuthorize(EmployerUserRole.Any)]
        [Route("agreements/{agreementId}/view")]
        public async Task<IActionResult> View(string agreementId, string hashedAccountId,
            FlashMessageViewModel flashMessage)
        {
            var agreement = await GetSignedAgreementViewModel(new GetEmployerAgreementRequest { AgreementId = agreementId, HashedAccountId = hashedAccountId, ExternalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName) });
            return View(agreement);
        }

        [HttpGet]
        [DasAuthorize(EmployerUserRole.Any)]
        [Route("agreements/unsigned/view")]
        public async Task<IActionResult> ViewUnsignedAgreements(string hashedAccountId)
        {
            var unsignedAgreementResponse = await _mediator.SendAsync(new GetNextUnsignedEmployerAgreementRequest { HashedAccountId = hashedAccountId, ExternalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName) });

            if (string.IsNullOrEmpty(unsignedAgreementResponse.HashedAgreementId)) return RedirectToAction(ControllerConstants.IndexActionName);

            return RedirectToAction(ControllerConstants.AboutYourAgreementActionName, new { agreementId = unsignedAgreementResponse.HashedAgreementId });
        }

        [HttpGet]
        [DasAuthorize(EmployerUserRole.Any)]
        [Route("agreements/{agreementId}/about-your-agreement")]
        public async Task<IActionResult> AboutYourAgreement(string agreementId, string hashedAccountId)
        {
            var agreement = await _orchestrator.GetById(
                agreementId,
                hashedAccountId,
                OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            return View(agreement.Data.EmployerAgreement.AgreementType == AgreementType.Levy ||
                agreement.Data.EmployerAgreement.AgreementType == AgreementType.Combined
                ? ControllerConstants.AboutYourAgreementViewName
                : ControllerConstants.AboutYourDocumentViewName, agreement);
        }

        [HttpGet]
        [DasAuthorize(EmployerUserRole.Any)]
        [Route("agreements/{agreementId}/sign-your-agreement")]
        public async Task<IActionResult> SignAgreement(GetEmployerAgreementRequest request)
        {
            request.ExternalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

            var viewModel = await GetSignedAgreementViewModel(request);
            var entities = await _mediator.SendAsync(new GetAccountLegalEntitiesCountByHashedAccountIdRequest { HashedAccountId = request.HashedAccountId });

            viewModel.LegalEntitiesCount = entities.LegalEntitiesCount;

            return View(viewModel);
        }

        [HttpPost]
        [DasAuthorize(EmployerUserRole.Any)]
        [Route("agreements/{agreementId}/sign")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sign(string agreementId, string hashedAccountId, int? choice)
        {
            var userInfo = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

            if (choice == null)
            {
                var agreement = await GetSignedAgreementViewModel(new GetEmployerAgreementRequest { AgreementId = agreementId, HashedAccountId = hashedAccountId, ExternalUserId = userInfo });

                ModelState.AddModelError(nameof(agreement.Choice), "Select whether you accept the agreement");

                return View(ControllerConstants.SignAgreementViewName, agreement);
            }

            if (choice == SignEmployerAgreementViewModel.ReviewAgreementLater)
            {
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName);
            }

            var response = await _orchestrator.SignAgreement(agreementId, hashedAccountId, userInfo, DateTime.UtcNow);

            if (response.Status == HttpStatusCode.Unauthorized)
            {
                return View(response);
            }

            var user = await _mediator.SendAsync(new GetUserByRefQuery { UserRef = userInfo });

            if (!string.IsNullOrWhiteSpace(user.User.CorrelationId))
            {
                var getProviderInvitationQueryResponse = await _mediator.SendAsync(new GetProviderInvitationQuery
                {
                    CorrelationId = Guid.Parse(user.User.CorrelationId)
                });

                if (getProviderInvitationQueryResponse.Result?.Status < InvitationComplete)
                {
                    return Redirect(_urlActionHelper.ProviderRelationshipsAction(RouteData, $"providers/invitation/{user.User.CorrelationId}"));
                }
            }


            if (response.Status == HttpStatusCode.OK)
            {
                ViewBag.CompanyName = response.Data.LegalEntityName;
                ViewBag.HasFurtherPendingAgreements = response.Data.HasFurtherPendingAgreements;
                return View(ControllerConstants.AcceptedEmployerAgreementViewName);
            }

            return RedirectToAction(ControllerConstants.SignAgreementActionName, new GetEmployerAgreementRequest { AgreementId = agreementId, ExternalUserId = userInfo, HashedAccountId = hashedAccountId });
        }

        [HttpGet]
        [DasAuthorize(EmployerUserRole.Any)]
        [Route("agreements/{agreementId}/agreement-pdf")]
        public async Task<IActionResult> GetPdfAgreement(string agreementId, string hashedAccountId)
        {
            var stream = await _orchestrator.GetPdfEmployerAgreement(hashedAccountId, agreementId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            if (stream.Data.PdfStream == null)
            {
                // ReSharper disable once Mvc.ViewNotResolved
                return View(stream);
            }

            return new FileStreamResult(stream.Data.PdfStream, ControllerConstants.PdfContentTypeName);
        }

        [HttpGet]
        [DasAuthorize(EmployerUserRole.Any)]
        [Route("agreements/{agreementId}/signed-agreement-pdf")]
        public async Task<IActionResult> GetSignedPdfAgreement(string agreementId, string hashedAccountId)
        {
            var stream = await _orchestrator.GetSignedPdfEmployerAgreement(hashedAccountId, agreementId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            if (stream.Data.PdfStream == null)
            {
                // ReSharper disable once Mvc.ViewNotResolved
                return View(stream);
            }

            return new FileStreamResult(stream.Data.PdfStream, ControllerConstants.PdfContentTypeName);
        }

        [HttpGet]
        [DasAuthorize(EmployerUserRole.Any)]
        [Route("agreements/{accountLegalEntityHashedId}/remove")]
        public async Task<IActionResult> ConfirmRemoveOrganisation(string accountLegalEntityHashedId, string hashedAccountId)
        {
            var model = await _orchestrator.GetConfirmRemoveOrganisationViewModel(accountLegalEntityHashedId, hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            return View(model.Data != null && model.Data.CanBeRemoved ? ControllerConstants.ConfirmRemoveOrganisationActionName : ControllerConstants.CannotRemoveOrganisationViewName, model);
        }

        [HttpPost]
        [DasAuthorize(EmployerUserRole.Any)]
        [Route("agreements/{accountLegalEntityHashedId}/remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveOrganisation(ConfirmOrganisationToRemoveViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ControllerConstants.ConfirmRemoveOrganisationViewName, new OrchestratorResponse<ConfirmOrganisationToRemoveViewModel> { Data = model });
            }
            if (!model.Remove.HasValue || !model.Remove.Value) return RedirectToAction(ControllerConstants.IndexActionName);

            var response = await _orchestrator.RemoveLegalAgreement(model, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            if (response.Status == HttpStatusCode.OK)
            {
                AddFlashMessageToCookie(response.FlashMessage);
                return RedirectToAction(ControllerConstants.IndexActionName);
            }

            AddFlashMessageToCookie(response.FlashMessage);
            return View(ControllerConstants.ConfirmRemoveOrganisationViewName, response);
        }

        private async Task<SignEmployerAgreementViewModel> GetSignedAgreementViewModel(GetEmployerAgreementRequest request)
        {
            var response = await _mediator.SendAsync(request);
            var viewModel = _mapper.Map<GetEmployerAgreementResponse, SignEmployerAgreementViewModel>(response);

            var signedAgreementResponse = await _mediator.SendAsync(new GetLastSignedAgreementRequest { AccountLegalEntityId = response.EmployerAgreement.LegalEntity.AccountLegalEntityId });
            viewModel.PreviouslySignedEmployerAgreement = _mapper.Map<EmployerAgreementView>(signedAgreementResponse.LastSignedAgreement);

            return viewModel;
        }

        [HttpGet]
        [Route("agreements/{agreementId}/whenDoYouWantToView")]
        public async Task<IActionResult> WhenDoYouWantToView(string agreementId, string hashedAccountId)
        {
            var userInfo = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId, userInfo);

            return View(new WhenDoYouWantToViewViewModel { EmployerAgreement = agreement.Data.EmployerAgreement });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("agreements/{agreementId}/whenDoYouWantToView")]
        public async Task<IActionResult> WhenDoYouWantToView(int? choice, string agreementId, string hashedAccountId)
        {
            switch (choice ?? 0)
            {
                case ViewAgreementNow: return RedirectToAction(ControllerConstants.SignAgreementActionName);
                case ViewAgreementLater: return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName);
                default:
                    {
                        var userInfo = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
                        var agreement = await _orchestrator.GetById(agreementId, hashedAccountId, userInfo);
                        return View(new WhenDoYouWantToViewViewModel { EmployerAgreement = agreement.Data.EmployerAgreement, InError = true });
                    }
            }
        }

        [HttpGet]
        [DasAuthorize(EmployerUserRole.Any)]
        [Route("organisations/{accountLegalEntityHashedId}/agreements")]
        public async Task<IActionResult> ViewAllAgreements(string hashedAccountId, string accountLegalEntityHashedId)
        {
            var model = await _orchestrator.GetOrganisationAgreements(accountLegalEntityHashedId);
            return View(model.Data);
        }
    }
}