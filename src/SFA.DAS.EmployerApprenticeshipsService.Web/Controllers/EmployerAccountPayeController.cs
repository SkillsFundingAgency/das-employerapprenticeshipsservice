using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class EmployerAccountPayeController : BaseController
    {
        private readonly IOwinWrapper _owinWrapper;
        private readonly EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;

        public EmployerAccountPayeController(IOwinWrapper owinWrapper,
            EmployerAccountPayeOrchestrator employerAccountPayeOrchestrator)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            if (employerAccountPayeOrchestrator == null)
                throw new ArgumentNullException(nameof(employerAccountPayeOrchestrator));
            _owinWrapper = owinWrapper;
            _employerAccountPayeOrchestrator = employerAccountPayeOrchestrator;
        }

        [HttpGet]
        public async Task<ActionResult> Index(long accountid)
        {
            var model = await _employerAccountPayeOrchestrator.Get(accountid, _owinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpGet]
        public ActionResult Details(long accountId, string empRef)
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Add(long accountId, bool? validationFailed)
        {
            var response = await _employerAccountPayeOrchestrator.CheckUserIsOwner(accountId, _owinWrapper.GetClaimValue("email"), Url.Action("Index", "EmployerAccountPaye", new { accountId }));

            if (validationFailed.HasValue && validationFailed.Value)
            {
                response.Data.ValidationFailed = true;
            }

            return View(response);
        }

        [HttpGet]
        public async Task<ActionResult> GetGateway(long accountId, bool confirmPayeVisibility)
        {
            if (confirmPayeVisibility)
            {
                return Redirect(await _employerAccountPayeOrchestrator.GetGatewayUrl(Url.Action("ConfirmPayeScheme", "EmployerAccountPaye", new { accountId }, Request.Url.Scheme)));
            }
            else
            {
                return Redirect(Url.Action("Add", new { accountId = accountId }));
            }
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmPayeScheme(long accountId)
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
        public async Task<ActionResult> ConfirmPayeScheme(AddNewPayeScheme model)
        {
            model.LegalEntities = await _employerAccountPayeOrchestrator.GetLegalEntities(model.AccountId, _owinWrapper.GetClaimValue(@"sub"));
            return View("ChooseCompany", model);
        }

        [HttpGet]
        public ActionResult ChooseCompany()
        {
            return RedirectToAction("GetGateway");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseCompany(int selectedCompanyId, AddNewPayeScheme model)
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
        public ActionResult AddNewLegalEntity(ConfirmNewPayeScheme model)
        {
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ChooseExistingLegalEntity(AddNewPayeScheme model)
        {
            return await ConfirmPayeScheme(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectCompany(ConfirmNewPayeScheme model)
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
        public async Task<ActionResult> Confirm(ConfirmNewPayeScheme model)
        {

            await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(model, _owinWrapper.GetClaimValue("sub"));

            TempData["successMessage"] = $"{model.PayeScheme} has been added to your account";

            return RedirectToAction("Index", "EmployerAccountPaye", new { accountId = model.AccountId });
        }

        [HttpGet]
        public ActionResult RemovePaye(long accountId, string payeRef)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePaye(RemovePayeScheme model)
        {
            model.UserId = _owinWrapper.GetClaimValue("sub");

            var result = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(model);

            if (result.Status != HttpStatusCode.OK)
            {
                return View(result);
            }

            return RedirectToAction("Index", "EmployerAccountPaye", new { model.AccountId });
        }
    }
}