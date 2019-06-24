﻿using System;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [AuthoriseActiveUser]
    [RoutePrefix("accounts")]
    public class EmployerAccountPayeController : BaseController
    {
        private const int AddPayeUsingGovernmentGateway = 1;
        private const int AddPayeUsingAorn = 2;

        private readonly EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;

        public EmployerAccountPayeController(
            IAuthenticationService owinWrapper, 
            EmployerAccountPayeOrchestrator employerAccountPayeOrchestrator,
            IAuthorizationService authorization, 
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage) : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));

            _employerAccountPayeOrchestrator = employerAccountPayeOrchestrator ?? throw new ArgumentNullException(nameof(employerAccountPayeOrchestrator));
        }

        [HttpGet]
        [Route("{HashedAccountId}/schemes")]
        public async Task<ActionResult> Index(string hashedAccountId)
        {
            var model = await _employerAccountPayeOrchestrator.Get(hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            var flashMessage = GetFlashMessageViewModelFromCookie();
            if (flashMessage != null)
            {
                model.FlashMessage = flashMessage;
            }

            return View(model);
        }

        [HttpGet]
        [Route("{HashedAccountId}/schemes/next")]
        public async Task<ActionResult> NextSteps(string hashedAccountId)
        {
            var model = await _employerAccountPayeOrchestrator.GetNextStepsViewModel(OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName), hashedAccountId);

            model.FlashMessage = GetFlashMessageViewModelFromCookie();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{HashedAccountId}/schemes/next")]
        public ActionResult NextSteps(int? choice)
        {
            switch (choice ?? 0)
            {
                case 1: return RedirectToAction(ControllerConstants.GatewayInformActionName);
                case 2: return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerAccountTransactionsControllerName);
                case 3: return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamActionName);
                default:
                    var model = new OrchestratorResponse<PayeSchemeNextStepsViewModel>
                    {
                        FlashMessage = GetFlashMessageViewModelFromCookie(),
                        Data = new PayeSchemeNextStepsViewModel { ErrorMessage = "You must select an option to continue." }
                    };
                    return View(model); //No option entered
            }
        }

        [HttpGet]
        [Route("{HashedAccountId}/schemes/{empRef}/details")]
        public async Task<ActionResult> Details(string hashedAccountId, string empRef)
        {
            empRef = empRef.FormatPayeFromUrl();

            var response = await _employerAccountPayeOrchestrator.GetPayeDetails(empRef, hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            return View(response);
        }

        [HttpGet]
        [Route("{HashedAccountId}/schemes/gatewayInform")]
        public async Task<ActionResult> GatewayInform(string hashedAccountId)
        {
            var response = await _employerAccountPayeOrchestrator.CheckUserIsOwner(
                hashedAccountId,
                OwinWrapper.GetClaimValue(ControllerConstants.EmailClaimKeyName),
                Url.Action(ControllerConstants.IndexActionName, ControllerConstants.EmployerAccountPayeControllerName, new { hashedAccountId }),
                Url.Action(ControllerConstants.GetGatewayActionName, ControllerConstants.EmployerAccountPayeControllerName, new { hashedAccountId }));

            return View(response);
        }

        [HttpGet]
        [Route("{HashedAccountId}/schemes/gateway")]
        public async Task<ActionResult> GetGateway(string hashedAccountId)
        {
            var url = await _employerAccountPayeOrchestrator.GetGatewayUrl(
                Url.Action(
                ControllerConstants.ConfirmPayeSchemeActionName,
                ControllerConstants.EmployerAccountPayeControllerName,
                null,
                HttpContext.Request.Url?.Scheme));

            return Redirect(url);
        }

        [HttpGet]
        [Route("{HashedAccountId}/schemes/confirm")]
        public async Task<ActionResult> ConfirmPayeScheme(string hashedAccountId)
        {
            var gatewayResponseModel = await _employerAccountPayeOrchestrator.GetPayeConfirmModel(
                    hashedAccountId, 
                    Request.Params[ControllerConstants.CodeKeyName], 
                    Url.Action(ControllerConstants.ConfirmPayeSchemeActionName, ControllerConstants.EmployerAccountPayeControllerName, new { hashedAccountId }, Request.Url?.Scheme), 
                    System.Web.HttpContext.Current?.Request.QueryString);

            if (gatewayResponseModel.Status == HttpStatusCode.NotAcceptable)
            {
                gatewayResponseModel.Status = HttpStatusCode.OK;

                var model = await _employerAccountPayeOrchestrator.Get(hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));
                model.FlashMessage = gatewayResponseModel.FlashMessage;

                return View(ControllerConstants.IndexActionName, model);
            }
            return View(gatewayResponseModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{HashedAccountId}/schemes/confirm")]
        public async Task<ActionResult> ConfirmPayeScheme(string hashedAccountId, AddNewPayeSchemeViewModel model)
        {
            var result = await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(model, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            if (result.Status != HttpStatusCode.OK)
            {
                return View(result);
            }

            var payeSchemeName = string.IsNullOrEmpty(model.PayeName) ? "this PAYE scheme" : model.PayeName;

            var flashMessage = new FlashMessageViewModel
            {
                Severity = FlashMessageSeverityLevel.Success,
                Headline = $"{model.PayeScheme} has been added",
                SubMessage = $"Levy funds from {payeSchemeName} will now credit your account",
                HiddenFlashMessageInformation = "page-paye-scheme-added"
            };
            AddFlashMessageToCookie(flashMessage);

            return RedirectToAction(ControllerConstants.NextStepsActionName, ControllerConstants.EmployerAccountPayeControllerName, new { model.HashedAccountId });
        }

        [HttpGet]
        [Route("{HashedAccountId}/schemes/{empRef}/remove")]
        public async Task<ActionResult> Remove(string hashedAccountId, string empRef)
        {
            var model = await _employerAccountPayeOrchestrator.GetRemovePayeSchemeModel(new RemovePayeSchemeViewModel
            {
                HashedAccountId = hashedAccountId,
                PayeRef = empRef.FormatPayeFromUrl(),
                UserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName)
            });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{HashedAccountId}/schemes/remove")]
        public async Task<ActionResult> RemovePaye(string hashedAccountId, RemovePayeSchemeViewModel model)
        {
            model.UserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

            if (model.RemoveScheme == 1)
            {
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerAccountPayeControllerName, new { model.HashedAccountId });
            }

            var result = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(model);

            if (result.Status != HttpStatusCode.OK)
            {
                return View(ControllerConstants.RemoveViewName, result);
            }

            model.PayeSchemeName = model.PayeSchemeName ?? string.Empty;

            var flashMessage = new FlashMessageViewModel
            {
                Severity = FlashMessageSeverityLevel.Success,
                Headline = $"You've removed {model.PayeRef}",
                SubMessage = model.PayeSchemeName,
                HiddenFlashMessageInformation = "page-paye-scheme-deleted"
            };

            AddFlashMessageToCookie(flashMessage);

            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerAccountPayeControllerName, new { model.HashedAccountId });
        }

        [HttpGet]
        [Route("schemes/waysToAdd")]
        public ViewResult WaysToAdd()
        {
            var model = new
            {
                HideHeaderSignInLink = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("schemes/waysToAdd")]
        public ActionResult WaysToAdd(int? choice)
        {
            switch (choice ?? 0)
            {
                case AddPayeUsingGovernmentGateway:
                    return RedirectToAction(ControllerConstants.GatewayInformActionName, ControllerConstants.EmployerAccountControllerName);
                case AddPayeUsingAorn:
                    return RedirectToAction(ControllerConstants.SearchUsingAornActionName, ControllerConstants.SearchPensionRegulatorControllerName);
                default:
                {
                    ViewBag.InError = true;
                    var model = new
                    {
                        HideHeaderSignInLink = true,
                    };

                    return View(model);
                }
            }
        }
    }
}