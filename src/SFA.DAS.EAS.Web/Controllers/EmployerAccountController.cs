using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("accounts")]
    [AuthoriseActiveUser]
    public class EmployerAccountController : BaseController
    {
        private readonly EmployerAccountOrchestrator _employerAccountOrchestrator;
        private readonly ILogger _logger;

        public EmployerAccountController(IOwinWrapper owinWrapper, EmployerAccountOrchestrator employerAccountOrchestrator,
            IFeatureToggle featureToggle, IMultiVariantTestingService multiVariantTestingService, ILogger logger, ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, featureToggle,multiVariantTestingService,flashMessage)
        {
            if (employerAccountOrchestrator == null)
                throw new ArgumentNullException(nameof(employerAccountOrchestrator));

            _employerAccountOrchestrator = employerAccountOrchestrator;
            _logger = logger;
        }
        
        [HttpGet]
        [Route("selectEmployer")]
        public ActionResult SelectEmployer()
        {
            _employerAccountOrchestrator.DeleteCookieData(HttpContext);

            return RedirectToAction("AddOrganisation", "EmployerAccountOrganisation");

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
                    BreadcrumbUrl = Url.Action("SelectEmployer", "EmployerAccount"),
                    ConfirmUrl = Url.Action("Gateway", "EmployerAccount"),
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
            return Redirect(await _employerAccountOrchestrator.GetGatewayUrl(Url.Action("GateWayResponse", "EmployerAccount", null, Request.Url.Scheme)));
        }

        [Route("gatewayResponse")]
        public async Task<ActionResult> GateWayResponse()
        {
            try
            {
                _logger.Info("Starting processing gateway response");

                var response = await _employerAccountOrchestrator.GetGatewayTokenResponse(Request.Params["code"], Url.Action("GateWayResponse", "EmployerAccount", null, Request.Url.Scheme), System.Web.HttpContext.Current?.Request.QueryString);
                if (response.Status != HttpStatusCode.OK)
                {
                    _logger.Warn($"Gateway response does not indicate success. Status = {response.Status}.");
                    response.Status = HttpStatusCode.OK;

                    AddFlashMessageToCookie(response.FlashMessage);
                    
                    return RedirectToAction("GatewayInform");
                }

                var email = OwinWrapper.GetClaimValue("email");
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
                return RedirectToAction("Summary");
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
            return RedirectToAction("Summary");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public async Task<ActionResult> CreateAccount()
        {
            var enteredData = _employerAccountOrchestrator.GetCookieData(HttpContext);

            if (enteredData == null)
                return RedirectToAction("SelectEmployer", "EmployerAccount");

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
                return RedirectToAction("summary");
            }

            var flashmessage = new FlashMessageViewModel
            {
                Headline = "Account created",
                HiddenFlashMessageInformation = enteredData.OrganisationType.ToString(),
                Severity = FlashMessageSeverityLevel.Complete
            };

            AddFlashMessageToCookie(flashmessage);
            
            return RedirectToAction("Index", "EmployerTeam", new { response.Data.EmployerAgreement.HashedAccountId });
        }

        [HttpGet]
        [Route("{HashedAccountId}/rename")]
        public async Task<ActionResult> RenameAccount(string hashedAccountId)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
            var vm = await _employerAccountOrchestrator.GetRenameEmployerAccountViewModel(hashedAccountId, userIdClaim);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{HashedAccountId}/rename")]
        public async Task<ActionResult> RenameAccount(RenameEmployerAccountViewModel vm)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
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
                
                return RedirectToAction("Index", "EmployerTeam");
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
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
            return userIdClaim ?? "";
        }
    }
}