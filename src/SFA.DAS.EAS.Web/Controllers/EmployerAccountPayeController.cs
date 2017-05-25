using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAccountPayeController : BaseController
    {
        private readonly EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;

        public EmployerAccountPayeController(IOwinWrapper owinWrapper,EmployerAccountPayeOrchestrator employerAccountPayeOrchestrator, 
            IFeatureToggle featureToggle, IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage) 
            : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            if (employerAccountPayeOrchestrator == null)
                throw new ArgumentNullException(nameof(employerAccountPayeOrchestrator));
           
            _employerAccountPayeOrchestrator = employerAccountPayeOrchestrator;
        }

        [HttpGet]
        [Route("schemes")]
        public async Task<ActionResult> Index(string hashedAccountId)
        {
            var model = await _employerAccountPayeOrchestrator.Get(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            var flashMessage = GetFlashMessageViewModelFromCookie();
            if (flashMessage != null)
            {
                model.FlashMessage = flashMessage;
            }

            return View(model);
        }

        [HttpGet]
        [Route("schemes/{empRef}/details")]
        public async Task<ActionResult> Details(string hashedAccountId, string empRef)
        {
            empRef = empRef.FormatPayeFromUrl();

            var response = await _employerAccountPayeOrchestrator.GetPayeDetails(empRef, hashedAccountId, OwinWrapper.GetClaimValue("sub"));
            
            return View(response);
        }


        [HttpGet]
        [Route("schemes/gatewayInform")]
        public async Task<ActionResult> GatewayInform(string hashedAccountId)
        {
            var response = await _employerAccountPayeOrchestrator.CheckUserIsOwner(
                hashedAccountId, 
                OwinWrapper.GetClaimValue("email"), 
                Url.Action("Index", "EmployerAccountPaye", new { hashedAccountId }),
                Url.Action("Index", "EmployerAccountPaye", new { hashedAccountId }),
                Url.Action("GetGateway", "EmployerAccountPaye", new { hashedAccountId }));
            
            return View(response);
        }
        
        [HttpGet]
        [Route("schemes/gateway")]
        public async Task<ActionResult> GetGateway(string hashedAccountId)
        {
            return Redirect(await _employerAccountPayeOrchestrator.GetGatewayUrl(Url.Action("ConfirmPayeScheme", "EmployerAccountPaye", new { hashedAccountId }, Request.Url.Scheme)));
        }

        [HttpGet]
        [Route("schemes/confirm")]
        public async Task<ActionResult> ConfirmPayeScheme(string hashedAccountId)
        {

            var gatewayResponseModel = await _employerAccountPayeOrchestrator.GetPayeConfirmModel(hashedAccountId, Request.Params["code"], Url.Action("ConfirmPayeScheme", "EmployerAccountPaye", new { hashedAccountId }, Request.Url.Scheme), System.Web.HttpContext.Current?.Request.QueryString);
            if (gatewayResponseModel.Status == HttpStatusCode.NotAcceptable)
            {
                gatewayResponseModel.Status = HttpStatusCode.OK;

                var model = await _employerAccountPayeOrchestrator.Get(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
                model.FlashMessage = gatewayResponseModel.FlashMessage;

                return View("Index", model);
            }
            return View(gatewayResponseModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("schemes/confirm")]
        public async Task<ActionResult> ConfirmPayeScheme(string hashedAccountId, AddNewPayeSchemeViewModel model)
        {
            var result = await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(model, OwinWrapper.GetClaimValue("sub"));

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

            return RedirectToAction("Index", "EmployerAccountPaye", new {model.HashedAccountId });
        }

        
        [HttpGet]
        [Route("schemes/{empRef}/remove")]
        public async Task<ActionResult> Remove(string hashedAccountId, string empRef)
        {
            var model = await _employerAccountPayeOrchestrator.GetRemovePayeSchemeModel(new RemovePayeSchemeViewModel
            {
                HashedAccountId = hashedAccountId,
                PayeRef = empRef.FormatPayeFromUrl(),
                UserId = OwinWrapper.GetClaimValue("sub")
            });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("schemes/remove")]
        public async Task<ActionResult> RemovePaye(string hashedAccountId, RemovePayeSchemeViewModel model)
        {
            model.UserId = OwinWrapper.GetClaimValue("sub");

            if (model.RemoveScheme == 1)
            {
                return RedirectToAction("Index", "EmployerAccountPaye", new {model.HashedAccountId });
            }

            var result = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(model);     

            if (result.Status != HttpStatusCode.OK)
            {
                return View("Remove",result);
            }

            model.PayeSchemeName = result?.Data?.PayeSchemeName ?? string.Empty;

            var flashMessage = new FlashMessageViewModel
            {
                Severity = FlashMessageSeverityLevel.Success,
                Headline = $"You've removed {model.PayeRef}",
                SubMessage = model.PayeSchemeName,
                HiddenFlashMessageInformation = "page-paye-scheme-deleted"
            };
            AddFlashMessageToCookie(flashMessage);
            
            return RedirectToAction("Index", "EmployerAccountPaye", new {model.HashedAccountId});
        }
    }
}