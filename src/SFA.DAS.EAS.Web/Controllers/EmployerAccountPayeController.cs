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
        public async Task<ActionResult> Index(string HashedAccountId)
        {
            var model = await _employerAccountPayeOrchestrator.Get(HashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

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
        public ActionResult Details(string HashedAccountId, string empRef)
        {
            empRef = empRef.FormatPayeFromUrl();

            return View();
        }


        [HttpGet]
        [Route("Schemes/GatewayInform")]
        public async Task<ActionResult> GatewayInform(string HashedAccountId)
        {
            var response = await _employerAccountPayeOrchestrator.CheckUserIsOwner(
                HashedAccountId, 
                OwinWrapper.GetClaimValue("email"), 
                Url.Action("Index", "EmployerAccountPaye", new { HashedAccountId }),
                Url.Action("Index", "EmployerAccountPaye", new { HashedAccountId }),
                Url.Action("GetGateway", "EmployerAccountPaye", new { HashedAccountId }));
            
            return View(response);
        }
        
        [HttpGet]
        [Route("Schemes/Gateway")]
        public async Task<ActionResult> GetGateway(string HashedAccountId)
        {
            return Redirect(await _employerAccountPayeOrchestrator.GetGatewayUrl(Url.Action("ConfirmPayeScheme", "EmployerAccountPaye", new { HashedAccountId }, Request.Url.Scheme)));
        }

        [HttpGet]
        [Route("Schemes/ConfirmPayeScheme")]
        public async Task<ActionResult> ConfirmPayeScheme(string HashedAccountId)
        {

            var gatewayResponseModel = await _employerAccountPayeOrchestrator.GetPayeConfirmModel(HashedAccountId, Request.Params["code"], Url.Action("ConfirmPayeScheme", "EmployerAccountPaye", new { HashedAccountId }, Request.Url.Scheme), System.Web.HttpContext.Current?.Request.QueryString);
            if (gatewayResponseModel.Status == HttpStatusCode.NotAcceptable)
            {
                gatewayResponseModel.Status = HttpStatusCode.OK;

                var model = await _employerAccountPayeOrchestrator.Get(HashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
                model.FlashMessage = gatewayResponseModel.FlashMessage;

                return View("Index", model);
            }
            return View(gatewayResponseModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Schemes/ConfirmPayeScheme")]
        public async Task<ActionResult> ConfirmPayeScheme(string HashedAccountId, AddNewPayeScheme model)
        {
            await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(model, OwinWrapper.GetClaimValue("sub"));

            TempData["payeSchemeAdded"] = "true";
            TempData["successMessage"] = $"You've added {model.PayeScheme}";
            TempData["subMessage"] = "Levy funds from this PAYE scheme will now credit your account";


            return RedirectToAction("Index", "EmployerAccountPaye", new { HashedAccountId = model.HashedAccountId });
        }

        
        [HttpGet]
        [Route("Schemes/{empRef}/Remove")]
        public async Task<ActionResult> Remove(string HashedAccountId, string empRef)
        {
            var model = await _employerAccountPayeOrchestrator.GetRemovePayeSchemeModel(new RemovePayeScheme
            {
                HashedAccountId = HashedAccountId,
                PayeRef = empRef.FormatPayeFromUrl(),
                UserId = OwinWrapper.GetClaimValue("sub")
            });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Schemes/RemovePaye")]
        public async Task<ActionResult> RemovePaye(string HashedAccountId, RemovePayeScheme model)
        {
            model.UserId = OwinWrapper.GetClaimValue("sub");

            if (model.RemoveScheme == 1)
            {
                return RedirectToAction("Index", "EmployerAccountPaye", new { HashedAccountId = model.HashedAccountId });
            }

            var result = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(model);     

            if (result.Status != HttpStatusCode.OK)
            {
                return View("Remove",result);
            }

            TempData["payeSchemeDeleted"] = "true";
            TempData["successMessage"] = $"You've removed {model.PayeRef}";
            TempData["subMessage"] = "No future levy funds will credit your account from this PAYE scheme";

            return RedirectToAction("Index", "EmployerAccountPaye", new {HashedAccountId = model.HashedAccountId});
        }
    }
}