using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{accountId}")]
    public class EmployerAccountPayeController : BaseController
    {
      
        private readonly EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;

        public EmployerAccountPayeController(IOwinWrapper owinWrapper,EmployerAccountPayeOrchestrator employerAccountPayeOrchestrator, 
            IFeatureToggle featureToggle, IUserWhiteList userWhiteList) 
            : base(owinWrapper, featureToggle, userWhiteList)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            if (employerAccountPayeOrchestrator == null)
                throw new ArgumentNullException(nameof(employerAccountPayeOrchestrator));
           
            _employerAccountPayeOrchestrator = employerAccountPayeOrchestrator;
        }

        [HttpGet]
        [Route("Schemes")]
        public async Task<ActionResult> Index(string accountId)
        {
            var model = await _employerAccountPayeOrchestrator.Get(accountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpGet]
        [Route("Schemes/{empRef}/Detail")]
        public ActionResult Details(string accountId, string empRef)
        {
            empRef = empRef.FormatPayeFromUrl();

            return View();
        }

        [HttpGet]
        [Route("Schemes/Add")]
        public async Task<ActionResult> Add(string accountId, bool? validationFailed)
        {
            var response = await _employerAccountPayeOrchestrator.CheckUserIsOwner(accountId, OwinWrapper.GetClaimValue("email"), Url.Action("Index", "EmployerAccountPaye", new { accountId }));

            if (validationFailed.HasValue && validationFailed.Value)
            {
                response.Data.ValidationFailed = true;
            }

            return View(response);
        }

        [HttpGet]
        [Route("Schemes/Gateway")]
        public async Task<ActionResult> GetGateway(string accountId, bool confirmPayeVisibility)
        {
            if (confirmPayeVisibility)
            {
                return Redirect(await _employerAccountPayeOrchestrator.GetGatewayUrl(Url.Action("ConfirmPayeScheme", "EmployerAccountPaye", new { accountId }, Request.Url.Scheme)));
            }
            else
            {
                return Redirect(Url.Action("Add", new { accountId,validationFailed=true }));
            }
        }

        [HttpGet]
        [Route("Schemes/ConfirmPayeScheme")]
        public async Task<ActionResult> ConfirmPayeScheme(string accountId)
        {

            var gatewayResponseModel = await _employerAccountPayeOrchestrator.GetPayeConfirmModel(accountId, Request.Params["code"], Url.Action("ConfirmPayeScheme", "EmployerAccountPaye", new { accountId }, Request.Url.Scheme), System.Web.HttpContext.Current?.Request.QueryString);
            if (gatewayResponseModel.Status == HttpStatusCode.NotAcceptable)
            {
                gatewayResponseModel.Status = HttpStatusCode.OK;

                var model = await _employerAccountPayeOrchestrator.Get(accountId, OwinWrapper.GetClaimValue(@"sub"));
                model.FlashMessage = gatewayResponseModel.FlashMessage;

                return View("Index", model);
            }
            return View(gatewayResponseModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Schemes/ConfirmPayeScheme")]
        public async Task<ActionResult> ConfirmPayeScheme(string accountId, AddNewPayeScheme model)
        {
            await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(model, OwinWrapper.GetClaimValue("sub"));

            TempData["successMessage"] = $"{model.PayeScheme} has been added to your account";

            return RedirectToAction("Index", "EmployerAccountPaye", new { accountId = model.HashedId });
        }

        
        [HttpGet]
        [Route("Schemes/{empRef}/Remove")]
        public async Task<ActionResult> Remove(string accountId, string empRef)
        {
            var model = await _employerAccountPayeOrchestrator.GetRemovePayeSchemeModel(new RemovePayeScheme
            {
                HashedId = accountId,
                PayeRef = empRef.FormatPayeFromUrl(),
                UserId = OwinWrapper.GetClaimValue("sub")
            });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Schemes/RemovePaye")]
        public async Task<ActionResult> RemovePaye(string accountId, RemovePayeScheme model)
        {
            model.UserId = OwinWrapper.GetClaimValue("sub");

            var result = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(model);

            if (result.Status != HttpStatusCode.OK)
            {
                return View("Remove",result);
            }

            TempData["successMessage"] = $"You've removed {model.PayeRef}";

            return RedirectToAction("Index", "EmployerAccountPaye", new {accountId = model.HashedId});
        }
    }
}