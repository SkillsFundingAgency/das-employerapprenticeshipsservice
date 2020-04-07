using System;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetProviderInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesCountByHashedAccountId;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAgreementController : BaseController
    {
        private const int InvitationComplete = 4;
        private const int ReviewAgreementLater = 1;
        private readonly EmployerAgreementOrchestrator _orchestrator;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private const int ViewAgreementNow = 1;
        private const int ViewAgreementLater = 2;

        public EmployerAgreementController(IAuthenticationService owinWrapper,
            EmployerAgreementOrchestrator orchestrator,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            IMediator mediator,
            IMapper mapper)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
            _mediator = mediator;
            _mapper = mapper;
        }

        public EmployerAgreementController(IAuthenticationService owinWrapper) : base(owinWrapper)
        {
        }

        [HttpGet]
        [Route("agreements")]
        public async Task<ActionResult> Index(string hashedAccountId, bool agreementSigned = false)
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
        [Route("agreements/{agreementId}/details")]
        public async Task<ActionResult> Details(string agreementId, string hashedAccountId,
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
        [Route("agreements/{agreementId}/view")]
        public async Task<ActionResult> View(string agreementId, string hashedAccountId,
            FlashMessageViewModel flashMessage)
        {
            var agreement = await GetSignedAgreementViewModel(new GetEmployerAgreementRequest { AgreementId = agreementId, HashedAccountId = hashedAccountId, ExternalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName) });

            return View(agreement);
        }

        [HttpGet]
        [Route("agreements/unsigned/view")]
        public async Task<ActionResult> ViewUnsignedAgreements(string hashedAccountId)
        {
            var unsignedAgreementResponse = await _mediator.SendAsync(new GetUnsignedEmployerAgreementRequest { HashedAccountId = hashedAccountId, ExternalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName) });

            if (string.IsNullOrEmpty(unsignedAgreementResponse.HashedAgreementId)) return RedirectToAction(ControllerConstants.IndexActionName);

            return RedirectToAction(ControllerConstants.AboutYourAgreementActionName, new { agreementId = unsignedAgreementResponse.HashedAgreementId });
        }

        [HttpGet]
        [Route("agreements/{agreementId}/about-your-agreement")]
        public async Task<ActionResult> AboutYourAgreement(string agreementId, string hashedAccountId)
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
        [Route("agreements/{agreementId}/sign-your-agreement")]
        public async Task<ActionResult> SignAgreement(GetEmployerAgreementRequest request)
        {
            request.ExternalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

            var viewModel = await GetSignedAgreementViewModel(request);
            var entities = await _mediator.SendAsync(new GetAccountLegalEntitiesCountByHashedAccountIdRequest { HashedAccountId = request.HashedAccountId });

            viewModel.LegalEntitiesCount = entities.LegalEntitiesCount;

            return View(viewModel);
        }

        [HttpPost]
        [Route("agreements/{agreementId}/sign")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Sign(string agreementId, string hashedAccountId, int? choice)
        {
            if (choice == ReviewAgreementLater)
            {
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName);
            }

            var userInfo = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            
            if (choice == null)
            {
                var agreement = await GetSignedAgreementViewModel(new GetEmployerAgreementRequest { AgreementId = agreementId, HashedAccountId = hashedAccountId, ExternalUserId = userInfo });
                agreement.NoChoiceSelected = true;
                return View(ControllerConstants.SignAgreementViewName, agreement);
            }

            var response = await _orchestrator.SignAgreement(agreementId, hashedAccountId, userInfo, DateTime.UtcNow);

            var user = await _mediator.SendAsync(new GetUserByRefQuery { UserRef = userInfo });

            if (!string.IsNullOrWhiteSpace(user.User.CorrelationId))
            {
                var getProviderInvitationQueryResponse = await _mediator.SendAsync(new GetProviderInvitationQuery
                {
                    CorrelationId = Guid.Parse(user.User.CorrelationId)
                });

                if (getProviderInvitationQueryResponse.Result?.Status < InvitationComplete)
                {
                    return Redirect(@Url.ProviderRelationshipsAction($"providers/invitation/{user.User.CorrelationId}"));
                }
            }

            if (response.Status == HttpStatusCode.OK)
            {
                FlashMessageViewModel flashMessage = new FlashMessageViewModel
                {
                    Headline = "Agreement(s) signed",
                    Severity = FlashMessageSeverityLevel.Success
                };

                ActionResult result;

                if (response.Data.SignedAgreementType == AgreementType.NonLevyExpressionOfInterest)
                {
                    flashMessage.Headline = "Memorandum of Understanding signed";
                    flashMessage.Message = "You’ve successfully signed the Memorandum of Understanding for your organisation.";
                    result = RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName);
                }
                else if (response.Data.HasFurtherPendingAgreements)
                {
                    flashMessage.Message = "You've successfully signed an organisation agreement. There are outstanding agreements to be signed. Review the list below to sign all remaining agreements.";

                    result = RedirectToAction(
                        ControllerConstants.IndexActionName,
                        ControllerConstants.EmployerAgreementControllerName,
                        new { hashedAccountId, agreementSigned = true }
                    );
                }
                else
                {
                    flashMessage.Headline = "Agreement accepted";
                    flashMessage.Message = "You’ve successfully accepted your organisation agreement(s)";
                    result = RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName);
                }

                AddFlashMessageToCookie(flashMessage);

                return result;
            }

            return RedirectToAction(ControllerConstants.SignAgreementActionName, new GetEmployerAgreementRequest { AgreementId = agreementId, ExternalUserId = userInfo, HashedAccountId = hashedAccountId });
        }

        [HttpGet]
        [Route("agreements/{agreementId}/agreement-pdf")]
        public async Task<ActionResult> GetPdfAgreement(string agreementId, string hashedAccountId)
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
        [Route("agreements/{agreementId}/signed-agreement-pdf")]
        public async Task<ActionResult> GetSignedPdfAgreement(string agreementId, string hashedAccountId)
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
        [Route("agreements/remove")]
        public async Task<ActionResult> GetOrganisationsToRemove(string hashedAccountId)
        {
            var model = await _orchestrator.GetLegalAgreementsToRemove(hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));
            return View(model);
        }

        [HttpGet]
        [Route("agreements/remove/{agreementId}")]
        public async Task<ActionResult> ConfirmRemoveOrganisation(string agreementId, string hashedAccountId)
        {
            var model = await _orchestrator.GetConfirmRemoveOrganisationViewModel(agreementId, hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            var flashMessage = GetFlashMessageViewModelFromCookie();
            if (flashMessage != null)
            {
                model.FlashMessage = flashMessage;
                model.Data.ErrorDictionary = model.FlashMessage.ErrorMessages;
            }

            return View(model);
        }

        [HttpPost]
        [Route("agreements/remove/{agreementId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveOrganisation(string hashedAccountId, string agreementId, ConfirmLegalAgreementToRemoveViewModel model)

        {
            var response = await _orchestrator.RemoveLegalAgreement(model, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            if (response.Status == HttpStatusCode.OK)
            {
                AddFlashMessageToCookie(response.FlashMessage);

                return RedirectToAction(ControllerConstants.IndexActionName, new { hashedAccountId });
            }

            if (response.Status == HttpStatusCode.BadRequest)
            {
                AddFlashMessageToCookie(response.FlashMessage);
                return RedirectToAction(ControllerConstants.ConfirmRemoveOrganisationActionName, new { hashedAccountId, agreementId });
            }

            return RedirectToAction(ControllerConstants.IndexActionName, new { hashedAccountId });
        }

        private async Task<SignEmployerAgreementViewModel> GetSignedAgreementViewModel(
            GetEmployerAgreementRequest request)
        {
            var response = await _mediator.SendAsync(request);
            var viewModel = _mapper.Map<GetEmployerAgreementResponse, SignEmployerAgreementViewModel>(response);

            var signedAgreementResponse = await _mediator.SendAsync(new GetLastSignedAgreementRequest
                {AccountLegalEntityId = response.EmployerAgreement.LegalEntity.AccountLegalEntityId});
            viewModel.PreviouslySignedEmployerAgreement =
                _mapper.Map<EmployerAgreementView>(signedAgreementResponse.LastSignedAgreement);
            return viewModel;
        }

        [HttpGet]
        [Route("agreements/{agreementId}/whenDoYouWantToView")]
        public async Task<ActionResult> WhenDoYouWantToView(string agreementId, string hashedAccountId)
        {
            var userInfo = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var agreement = await _orchestrator.GetById(agreementId, hashedAccountId, userInfo);

            return View(new WhenDoYouWantToViewViewModel{ EmployerAgreement = agreement.Data.EmployerAgreement });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("agreements/{agreementId}/whenDoYouWantToView")]
        public async Task<ActionResult> WhenDoYouWantToView(int? choice, string agreementId, string hashedAccountId)
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
        [Route("agreements/{accountLegalEntityHashedId}/viewallagreements")]
        public async Task<ActionResult> ViewAllAgreements(string hashedAccountId, string accountLegalEntityHashedId)
        {
            var model = await _orchestrator.GetOrganisationAgreements(accountLegalEntityHashedId);
            return View(model);
        }
    }
}