using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class EmployerAccountPayeController : Controller
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
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            var schemes = await _employerAccountPayeOrchestrator.Get(accountid, userIdClaim);

            return View(new EmployerAccountPayeListViewModel
            {
                AccountId = accountid,
                PayeSchemes = schemes
            });
        }

        [HttpGet]
        public async Task<ActionResult> Details(long accountId, string empRef)
        {
            if (string.IsNullOrWhiteSpace(_owinWrapper.GetClaimValue(@"sub")))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpGet]
        public ActionResult Add(long accountId)
        {
            if (string.IsNullOrWhiteSpace(_owinWrapper.GetClaimValue(@"sub")))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(accountId);
        }

        [HttpGet]
        public async Task<ActionResult> GetGateway(long accountId)
        {
            return Redirect(await _employerAccountPayeOrchestrator.GetGatewayUrl(Url.Action("ConfirmPayeScheme", "EmployerAccountPaye", new { accountId }, Request.Url.Scheme)));
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmPayeScheme(long accountId)
        {
            var response = await _employerAccountPayeOrchestrator.GetGatewayTokenResponse(Request.Params["code"], Url.Action("ConfirmPayeScheme", "EmployerAccountPaye", new { accountId }, Request.Url.Scheme));
            var gatewayResponseModel = _employerAccountPayeOrchestrator.GetPayeConfirmModel(accountId, response);

            return View(gatewayResponseModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmPayeScheme(AddNewPayeScheme model)
        {
            model.LegalEntities = await _employerAccountPayeOrchestrator.GetLegalEntities(model.AccountId, _owinWrapper.GetClaimValue(@"sub"));
            return View("ChooseCompany", model);
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
            
            model.LegalEntityCode = result.CompanyNumber;
            model.LegalEntityDateOfIncorporation = result.DateOfIncorporation;
            model.LegalEntityName = result.CompanyName;
            model.LegalEntityRegisteredAddress = result.RegisteredAddress;
            
            return View("Confirm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Confirm(ConfirmNewPayeScheme model)
        {

            await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(model, _owinWrapper.GetClaimValue("sub"));

            //TODO add the success message

            return RedirectToAction("Index", "EmployerAccountPaye", new { accountId = model.AccountId });
        }

    }
}