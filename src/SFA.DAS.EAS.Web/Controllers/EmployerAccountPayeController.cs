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
    [RoutePrefix("accounts/{HashedAccountId}")]
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
        public async Task<ActionResult> Index(string hashedAccountId)
        {
            var model = await _employerAccountPayeOrchestrator.Get(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            if (TempData["successMessage"] != null)
            {
                model.FlashMessage = new FlashMessageViewModel
                {
                    Headline = TempData["successMessage"].ToString(),
                    Message = TempData["subMessage"].ToString()
                };
                TempData.Remove("subMessage");
                TempData.Remove("successMessage");
            }

            return View(model);
        }

        [HttpGet]
        [Route("Schemes/{empRef}/Detail")]
        public ActionResult Details(string hashedAccountId, string empRef)
        {
            empRef = empRef.FormatPayeFromUrl();

            var response = await _employerAccountPayeOrchestrator.GetPayeDetails(empRef, accountId, OwinWrapper.GetClaimValue("sub"));
            
            return View(response);
        }


        [HttpGet]
        [Route("Schemes/GatewayInform")]
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
        [Route("Schemes/Gateway")]
        public async Task<ActionResult> GetGateway(string hashedAccountId)
        {
            return Redirect(await _employerAccountPayeOrchestrator.GetGatewayUrl(Url.Action("ConfirmPayeScheme", "EmployerAccountPaye", new { hashedAccountId }, Request.Url.Scheme)));
        }

        [HttpGet]
        [Route("Schemes/ConfirmPayeScheme")]
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
        [Route("Schemes/ConfirmPayeScheme")]
        public async Task<ActionResult> ConfirmPayeScheme(string hashedAccountId, AddNewPayeScheme model)
        {
            await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(model, OwinWrapper.GetClaimValue("sub"));

            TempData["payeSchemeAdded"] = "true";
            TempData["successMessage"] = $"You've added {model.PayeScheme}";
            TempData["subMessage"] = "Levy funds from this PAYE scheme will now credit your account";


            return RedirectToAction("Index", "EmployerAccountPaye", new {model.HashedAccountId });
        }

        
        [HttpGet]
        [Route("Schemes/{empRef}/Remove")]
        public async Task<ActionResult> Remove(string hashedAccountId, string empRef)
        {
            var model = await _employerAccountPayeOrchestrator.GetRemovePayeSchemeModel(new RemovePayeScheme
            {
                HashedAccountId = hashedAccountId,
                PayeRef = empRef.FormatPayeFromUrl(),
                UserId = OwinWrapper.GetClaimValue("sub")
            });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Schemes/RemovePaye")]
        public async Task<ActionResult> RemovePaye(string hashedAccountId, RemovePayeScheme model)
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

            TempData["payeSchemeDeleted"] = "true";
            TempData["successMessage"] = $"You've removed {model.PayeRef}";
            TempData["subMessage"] = "No future levy funds will credit your account from this PAYE scheme";

            return RedirectToAction("Index", "EmployerAccountPaye", new {model.HashedAccountId});
        }
    }
}