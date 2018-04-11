using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.NLog.Logger;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("accounts")]
    [AuthoriseActiveUser]
    public class EmployerAccountController : BaseController
    {
        private readonly EmployerAccountOrchestrator _employerAccountOrchestrator;
        private readonly ILog _logger;

        public EmployerAccountController(IAuthenticationService owinWrapper, EmployerAccountOrchestrator employerAccountOrchestrator,
            IAuthorizationService authorization, IMultiVariantTestingService multiVariantTestingService, ILog logger, 
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper,multiVariantTestingService,flashMessage)
        {
            if (employerAccountOrchestrator == null)
            {
                throw new ArgumentNullException(nameof(employerAccountOrchestrator));
            }

            _employerAccountOrchestrator = employerAccountOrchestrator;
            _logger = logger;
        }
        
        // Not sure if this is used anymore, leaving here for now to see if it breaks when the automated tests are run
        // the redirect won't go anywhere so if it is called from somewhere we should expect a 404
        [HttpGet]
        [Route("selectEmployer")]
        public ActionResult SelectEmployer()
        {
            _employerAccountOrchestrator.DeleteCookieData(HttpContext);

            return RedirectToAction(ControllerConstants.AddOrganisationViewName, ControllerConstants.OrganisationSharedControllerName);

        }

        [HttpGet]
        [Route("gatewayInform")]
        public ActionResult GatewayInform()
        {
            var gatewayInformViewModel = new OrchestratorResponse<GatewayInformViewModel>
            {
                Data = new GatewayInformViewModel
                {
                    BreadcrumbDescription = "Back to Your User Profile",
                    ConfirmUrl = Url.Action(ControllerConstants.GatewayViewName, ControllerConstants.EmployerAccountControllerName),
                },
             
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
            return Redirect(await _employerAccountOrchestrator.GateWayUrlHelper(ControllerConstants.GateWayResponseActionName, ControllerConstants.EmployerAccountControllerName, HttpContext.Request.Url.Scheme, Url));
        }

        [Route("gatewayResponse")]
        public async Task<ActionResult> GateWayResponse()
        {
            try
            {
                _logger.Info("Starting processing gateway response");

                var response = await _employerAccountOrchestrator.GetGatewayTokenResponse(Request.Params[ControllerConstants.CodeKeyName], Url.Action(ControllerConstants.GateWayResponseActionName, ControllerConstants.EmployerAccountControllerName, null, Request.Url.Scheme), System.Web.HttpContext.Current?.Request.QueryString);
                if (response.Status != HttpStatusCode.OK)
                {
                    _logger.Warn($"Gateway response does not indicate success. Status = {response.Status}.");
                    response.Status = HttpStatusCode.OK;

                    AddFlashMessageToCookie(response.FlashMessage);
                    
                    return RedirectToAction(ControllerConstants.GatewayInformActionName);
                }

                var email = OwinWrapper.GetClaimValue(ControllerConstants.EmailClaimKeyName);
                _logger.Info($"Gateway response is for user {email}");

                var empref = await _employerAccountOrchestrator.GetHmrcEmployerInformation(response.Data.AccessToken, email);
                _logger.Info($"Gateway response is for empref {empref.Empref} \n {JsonConvert.SerializeObject(empref)}");

                var enteredData = _employerAccountOrchestrator.GetCookieData(HttpContext);

                enteredData.EmployerRefName = empref.EmployerLevyInformation?.Employer?.Name?.EmprefAssociatedName ?? "";
                enteredData.PayeReference = empref.Empref;
                enteredData.AccessToken = response.Data.AccessToken;
                enteredData.RefreshToken = response.Data.RefreshToken;
                enteredData.EmpRefNotFound = empref.EmprefNotFound;
                _employerAccountOrchestrator.UpdateCookieData(HttpContext, enteredData);

                _logger.Info("Finished processing gateway response");
                return RedirectToAction(ControllerConstants.SummaryActionName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error processing Gateway response - {ex.Message}");
                throw;
            }
        }
        
        [HttpGet]
        [Route("summary")]
        public ViewResult Summary()
        {
            var result = _employerAccountOrchestrator.GetSummaryViewModel(HttpContext);
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("legalAgreement")]
        public ActionResult LegalAgreement()
        {
            return View();
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
            var enteredData = _employerAccountOrchestrator.GetCookieData(HttpContext);

            if (enteredData == null)
                return RedirectToAction(ControllerConstants.SelectEmployerActionName, ControllerConstants.EmployerAccountControllerName);

            var request = new CreateAccountViewModel
            {
                UserId = GetUserId(),
                OrganisationType = enteredData.OrganisationType,
                OrganisationReferenceNumber = enteredData.OrganisationReferenceNumber,
                OrganisationName = enteredData.OrganisationName,
                OrganisationAddress = enteredData.OrganisationRegisteredAddress,
                OrganisationDateOfInception = enteredData.OrganisationDateOfInception,
                PayeReference = enteredData.PayeReference,
                AccessToken = enteredData.AccessToken,
                RefreshToken = enteredData.RefreshToken,
                OrganisationStatus = string.IsNullOrWhiteSpace(enteredData.OrganisationStatus) ? null : enteredData.OrganisationStatus,
                EmployerRefName = enteredData.EmployerRefName,
                PublicSectorDataSource = enteredData.PublicSectorDataSource,
                Sector = enteredData.Sector
            };

            var response = await _employerAccountOrchestrator.CreateAccount(request, HttpContext);

            if (response.Status == HttpStatusCode.BadRequest)
            {
                response.Status = HttpStatusCode.OK;
                response.FlashMessage = new FlashMessageViewModel { Headline = "There was a problem creating your account" };
                return RedirectToAction(ControllerConstants.SummaryActionName);
            }

            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamActionName, new { response.Data.EmployerAgreement.HashedAccountId });
        }

        [HttpGet]
        [Route("{HashedAccountId}/rename")]
        public async Task<ActionResult> RenameAccount(string hashedAccountId)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName);
            var vm = await _employerAccountOrchestrator.GetRenameEmployerAccountViewModel(hashedAccountId, userIdClaim);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{HashedAccountId}/rename")]
        public async Task<ActionResult> RenameAccount(RenameEmployerAccountViewModel vm)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName);
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
                
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamActionName);
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

        private string GetUserId()
        {
            var userIdClaim = OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName);
            return userIdClaim ?? "";
        }

      
    }
}