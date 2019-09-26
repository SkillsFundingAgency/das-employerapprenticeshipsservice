﻿using System;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.PayeRefData;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Models;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [RoutePrefix("accounts")]
    [DasAuthorize]
    public class EmployerAccountController : BaseController
    {
        private readonly EmployerAccountOrchestrator _employerAccountOrchestrator;
        private readonly ILog _logger;
        private readonly IMediator _mediatr;
        private ICookieStorageService<HashedAccountIdModel> _accountCookieStorage;
        private const int AddPayeLater = 1;
        private const int AddPayeNow = 2;
        private readonly ICookieStorageService<ReturnUrlModel> _returnUrlCookieStorageService;
        private readonly string _hashedAccountIdCookieName;
        private const string ReturnUrlCookieName = "SFA.DAS.EmployerAccounts.Web.Controllers.ReturnUrlCookie";

        public EmployerAccountController(IAuthenticationService owinWrapper,
            EmployerAccountOrchestrator employerAccountOrchestrator,
            IMultiVariantTestingService multiVariantTestingService,
            ILog logger,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            IMediator mediatr,
            ICookieStorageService<ReturnUrlModel> returnUrlCookieStorageService,
            ICookieStorageService<HashedAccountIdModel> accountCookieStorage)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _employerAccountOrchestrator = employerAccountOrchestrator;
            _logger = logger;
            _mediatr = mediatr ?? throw new ArgumentNullException(nameof(mediatr));
            _returnUrlCookieStorageService = returnUrlCookieStorageService;
            _accountCookieStorage = accountCookieStorage;
            _hashedAccountIdCookieName = typeof(HashedAccountIdModel).FullName;
        }

        [HttpGet]
        [Route("{HashedAccountId}/gatewayInform", Order = 0)]
        [Route("gatewayInform", Order = 1)]
        public ActionResult GatewayInform(string hashedAccountId)
        {
            if (!string.IsNullOrWhiteSpace(hashedAccountId))
            {
                _accountCookieStorage.Delete(_hashedAccountIdCookieName);
                
                    _accountCookieStorage.Create(
                        new HashedAccountIdModel { Value = hashedAccountId },
                        _hashedAccountIdCookieName);
            }

            var gatewayInformViewModel = new OrchestratorResponse<GatewayInformViewModel>
            {
                Data = new GatewayInformViewModel
                {
                    BreadcrumbDescription = "Back to Your User Profile",
                    ConfirmUrl = Url.Action(ControllerConstants.GatewayViewName, ControllerConstants.EmployerAccountControllerName),
                }
            };

            var flashMessageViewModel = GetFlashMessageViewModelFromCookie();

            if (flashMessageViewModel != null)
            {
                gatewayInformViewModel.FlashMessage = flashMessageViewModel;
            }

            return View(gatewayInformViewModel);
        }

        [HttpGet]
        [Route("gateway")]
        public async Task<ActionResult> Gateway()
        {
            var url = await _employerAccountOrchestrator.GetGatewayUrl(
                Url.Action(ControllerConstants.GateWayResponseActionName,
                    ControllerConstants.EmployerAccountControllerName, 
                    null, 
                    HttpContext.Request.Url?.Scheme));

            return Redirect(url);
        }

        [Route("gatewayResponse")]
        public async Task<ActionResult> GateWayResponse()
        {
            try
            {
                _logger.Info("Starting processing gateway response");

                if (Request.Url == null)
                    return RedirectToAction(ControllerConstants.SearchPensionRegulatorActionName, ControllerConstants.SearchPensionRegulatorControllerName);

                var response = await _employerAccountOrchestrator.GetGatewayTokenResponse(
                    Request.Params[ControllerConstants.CodeKeyName],
                    Url.Action(ControllerConstants.GateWayResponseActionName, ControllerConstants.EmployerAccountControllerName, null, Request.Url.Scheme),
                    System.Web.HttpContext.Current?.Request.QueryString);

                if (response.Status != HttpStatusCode.OK)
                {
                    _logger.Warn($"Gateway response does not indicate success. Status = {response.Status}.");
                    response.Status = HttpStatusCode.OK;

                    AddFlashMessageToCookie(response.FlashMessage);

                    return RedirectToAction(ControllerConstants.GatewayInformActionName);
                }

                var externalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
                _logger.Info($"Gateway response is for user identity ID {externalUserId}");

                var email = OwinWrapper.GetClaimValue(ControllerConstants.EmailClaimKeyName);
                var empref = await _employerAccountOrchestrator.GetHmrcEmployerInformation(response.Data.AccessToken, email);
                _logger.Info($"Gateway response is for empref {empref.Empref} \n {JsonConvert.SerializeObject(empref)}");

                await _mediatr.SendAsync(new SavePayeRefData(new EmployerAccountPayeRefData
                {
                    EmployerRefName = empref.EmployerLevyInformation?.Employer?.Name?.EmprefAssociatedName ?? "",
                    PayeReference = empref.Empref,
                    AccessToken = response.Data.AccessToken,
                    RefreshToken = response.Data.RefreshToken,
                    EmpRefNotFound = empref.EmprefNotFound,
                }));
                
                _logger.Info("Finished processing gateway response");

                if (string.IsNullOrEmpty(empref.Empref) || empref.EmprefNotFound)
                {
                    return RedirectToAction(ControllerConstants.PayeErrorActionName,
                        new
                        {
                            NotFound = empref.EmprefNotFound
                        });
                }

                return RedirectToAction(ControllerConstants.SearchPensionRegulatorActionName, ControllerConstants.SearchPensionRegulatorControllerName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error processing Gateway response - {ex.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("getApprenticeshipFunding")]
        public ActionResult GetApprenticeshipFunding()
        {
            var model = new
            {
                HideHeaderSignInLink = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("getApprenticeshipFunding")]
        public async Task<ActionResult> GetApprenticeshipFunding(int? choice)
        {
            switch (choice ?? 0)
            {
                case AddPayeLater: return RedirectToAction(ControllerConstants.SkipRegistrationActionName);
                case AddPayeNow: return RedirectToAction(ControllerConstants.WaysToAddPayeSchemeActionName, ControllerConstants.EmployerAccountPayeControllerName);
                default:
                {
                    var model = new
                    {
                        HideHeaderSignInLink = true,
                        InError = true
                    };

                    return View(model);
                }
            }
        }

        [HttpGet]
        [Route("skipRegistration")]
        public async Task<ActionResult> SkipRegistration()
        {
            var request = new CreateUserAccountViewModel
            {
                UserId = GetUserId(),
                OrganisationName = "MY ACCOUNT"
            };

            var response = await _employerAccountOrchestrator.CreateMinimalUserAccountForSkipJourney(request, HttpContext);
            var returnUrlCookie = _returnUrlCookieStorageService.Get(ReturnUrlCookieName);
            _returnUrlCookieStorageService.Delete(ReturnUrlCookieName);
            if (returnUrlCookie != null && !returnUrlCookie.Value.IsNullOrWhiteSpace())
                return Redirect(returnUrlCookie.Value);

            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName, new { hashedAccountId = response.Data.HashedId });
        }

        [HttpGet]
        [Route("payeerror")]
        public ViewResult PayeError(bool? notFound)
        {
            ViewBag.NotFound = notFound ?? false;
            return View();
        }

        [HttpGet]
        [Route("summary")]
        public ViewResult Summary()
        {
            var result = _employerAccountOrchestrator.GetSummaryViewModel(HttpContext);
            return View(result);
        }

        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            return RedirectToAction(ControllerConstants.SummaryActionName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public async Task<ActionResult> CreateAccount()
        {
            var enteredData = _employerAccountOrchestrator.GetCookieData();

            if (enteredData == null)
            {
                // N.B CHANGED THIS FROM SelectEmployer which went nowhere.
                _employerAccountOrchestrator.DeleteCookieData();

                return RedirectToAction(ControllerConstants.SearchForOrganisationActionName, ControllerConstants.SearchOrganisationControllerName);
            }

            var request = new CreateAccountModel
            {
                UserId = GetUserId(),
                OrganisationType = enteredData.EmployerAccountOrganisationData.OrganisationType,
                OrganisationReferenceNumber = enteredData.EmployerAccountOrganisationData.OrganisationReferenceNumber,
                OrganisationName = enteredData.EmployerAccountOrganisationData.OrganisationName,
                OrganisationAddress = enteredData.EmployerAccountOrganisationData.OrganisationRegisteredAddress,
                OrganisationDateOfInception = enteredData.EmployerAccountOrganisationData.OrganisationDateOfInception,
                PayeReference = enteredData.EmployerAccountPayeRefData.PayeReference,
                AccessToken = enteredData.EmployerAccountPayeRefData.AccessToken,
                RefreshToken = enteredData.EmployerAccountPayeRefData.RefreshToken,
                OrganisationStatus = string.IsNullOrWhiteSpace(enteredData.EmployerAccountOrganisationData.OrganisationStatus) ? null : enteredData.EmployerAccountOrganisationData.OrganisationStatus,
                EmployerRefName = enteredData.EmployerAccountPayeRefData.EmployerRefName,
                PublicSectorDataSource = enteredData.EmployerAccountOrganisationData.PublicSectorDataSource,
                Sector = enteredData.EmployerAccountOrganisationData.Sector,
                HashedAccountId = _accountCookieStorage.Get(_hashedAccountIdCookieName),
                Aorn = enteredData.EmployerAccountPayeRefData.AORN
            };

            var response = await _employerAccountOrchestrator.CreateOrUpdateAccount(request, HttpContext);

            if (response.Status == HttpStatusCode.BadRequest)
            {
                response.Status = HttpStatusCode.OK;
                response.FlashMessage = new FlashMessageViewModel { Headline = "There was a problem creating your account" };
                return RedirectToAction(ControllerConstants.SummaryActionName);
            }
            
            _employerAccountOrchestrator.DeleteCookieData();

            var returnUrlCookie = _returnUrlCookieStorageService.Get(ReturnUrlCookieName);
            _accountCookieStorage.Delete(_hashedAccountIdCookieName);

            _returnUrlCookieStorageService.Delete(ReturnUrlCookieName);

            if (returnUrlCookie != null && !returnUrlCookie.Value.IsNullOrWhiteSpace())
                return Redirect(returnUrlCookie.Value);

            return RedirectToAction(ControllerConstants.AboutYourAgreementActionName, ControllerConstants.EmployerAgreementControllerName, new { hashedAccountId = response.Data.EmployerAgreement.HashedAccountId, agreementId = response.Data.EmployerAgreement.HashedAgreementId });
        }

        [HttpGet]
        [Route("{HashedAccountId}/rename")]
        public async Task<ActionResult> RenameAccount(string hashedAccountId)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var vm = await _employerAccountOrchestrator.GetRenameEmployerAccountViewModel(hashedAccountId, userIdClaim);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{HashedAccountId}/rename")]
        public async Task<ActionResult> RenameAccount(RenameEmployerAccountViewModel vm)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var response = await _employerAccountOrchestrator.RenameEmployerAccount(vm, userIdClaim);

            if (response.Status == HttpStatusCode.OK)
            {
                var flashmessage = new FlashMessageViewModel
                {
                    Headline = "Account renamed",
                    Message = "You successfully updated the account name",
                    Severity = FlashMessageSeverityLevel.Success
                };

                AddFlashMessageToCookie(flashmessage);

                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName);
            }

            var errorResponse = new OrchestratorResponse<RenameEmployerAccountViewModel>();

            if (response.Status == HttpStatusCode.BadRequest)
            {
                vm.ErrorDictionary = response.FlashMessage.ErrorMessages;
            }

            errorResponse.Data = vm;
            errorResponse.FlashMessage = response.FlashMessage;
            errorResponse.Status = response.Status;

            return View(errorResponse);
        }

        [HttpGet]
        [Route("amendOrganisation")]
        public ActionResult AmendOrganisation()
        {
            var employerAccountData = _employerAccountOrchestrator.GetCookieData();

            if (employerAccountData.EmployerAccountOrganisationData.OrganisationType == OrganisationType.PensionsRegulator && employerAccountData.EmployerAccountOrganisationData.PensionsRegulatorReturnedMultipleResults)
            {
                if (!string.IsNullOrWhiteSpace(employerAccountData.EmployerAccountPayeRefData.AORN))
                {
                    return RedirectToAction(
                        ControllerConstants.SearchUsingAornActionName,
                        ControllerConstants.SearchPensionRegulatorControllerName,
                        new
                        {
                            Aorn = employerAccountData.EmployerAccountPayeRefData.AORN,
                            payeRef = employerAccountData.EmployerAccountPayeRefData.PayeReference
                        });
                }

                return RedirectToAction(ControllerConstants.SearchPensionRegulatorActionName, ControllerConstants.SearchPensionRegulatorControllerName);
            }

            return RedirectToAction(ControllerConstants.SearchForOrganisationActionName, ControllerConstants.SearchOrganisationControllerName);
        }

        private string GetUserId()
        {
            var userIdClaim = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            return userIdClaim ?? "";
        }
    }
}