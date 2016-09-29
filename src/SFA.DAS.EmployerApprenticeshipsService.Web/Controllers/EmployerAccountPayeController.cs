using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
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
        public async Task<ActionResult> Index(string accountid)
        {
            var model = await _employerAccountPayeOrchestrator.Get(accountid, OwinWrapper.GetClaimValue(@"sub"));

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
                return Redirect(Url.Action("Add", new { accountId = accountId,validationFailed=true }));
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
                return View("ErrorConfrimPayeScheme", gatewayResponseModel);
            }
            return View(gatewayResponseModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Schemes/ConfirmPayeScheme")]
        public async Task<ActionResult> ConfirmPayeScheme(string accountId, AddNewPayeScheme model)
        {
            model.LegalEntities = await _employerAccountPayeOrchestrator.GetLegalEntities(model.HashedId, OwinWrapper.GetClaimValue(@"sub"));
            return View("ChooseCompany", model);
        }

        [HttpGet]
        [Route("Schemes/ChooseCompany")]
        public ActionResult ChooseCompany(string accountId)
        {
            return RedirectToAction("GetGateway");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Schemes/ChooseCompany")]
        public ActionResult ChooseCompany(string accountId, int selectedCompanyId, AddNewPayeScheme model)
        {
            if (selectedCompanyId == -1)
            {
                var modelConfirm = new ConfirmNewPayeScheme(model);

                return RedirectToAction("AddNewLegalEntity", modelConfirm);
            }
            else
            {
                var legalEntity = model.LegalEntities.SingleOrDefault(c => c.Id == selectedCompanyId);
                var modelConfirm = new ConfirmNewPayeScheme(model)
                {
                    LegalEntityCode = legalEntity.Code,
                    LegalEntityDateOfIncorporation = legalEntity.DateOfIncorporation,
                    LegalEntityName = legalEntity.Name,
                    LegalEntityRegisteredAddress = legalEntity.RegisteredAddress,
                    LegalEntityId = legalEntity.Id
                };

                return View("Confirm", modelConfirm);
            }

        }

        [HttpGet]
        [Route("Schemes/AddNewLegalEntity")]
        public ActionResult AddNewLegalEntity(string accountId, ConfirmNewPayeScheme model)
        {
            return View(model);
        }

        [HttpGet]
        [Route("Schemes/ChooseExistingLegalEntity")]
        public async Task<ActionResult> ChooseExistingLegalEntity(string accountId, AddNewPayeScheme model)
        {
            return await ConfirmPayeScheme(model.HashedId,model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Schemes/SelectCompany")]
        public async Task<ActionResult> SelectCompany(string accountId, ConfirmNewPayeScheme model)
        {

            var result = await _employerAccountPayeOrchestrator.GetCompanyDetails(new SelectEmployerModel { EmployerRef = model.LegalEntityCode });

            model.LegalEntityCode = result.Data.CompanyNumber;
            model.LegalEntityDateOfIncorporation = result.Data.DateOfIncorporation;
            model.LegalEntityName = result.Data.CompanyName;
            model.LegalEntityRegisteredAddress = result.Data.RegisteredAddress;

            return View("Confirm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Schemes/Confirm")]
        public async Task<ActionResult> Confirm(string accountId, ConfirmNewPayeScheme model)
        {
            await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(model, OwinWrapper.GetClaimValue("sub"));

            TempData["successMessage"] = $"{model.PayeScheme} has been added to your account";

            return RedirectToAction("Index", "EmployerAccountPaye", new {accountId = model.HashedId});
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