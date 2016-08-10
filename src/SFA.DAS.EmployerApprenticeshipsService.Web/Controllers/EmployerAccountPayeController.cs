using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
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
            var response = await _employerAccountPayeOrchestrator.GetGatewayTokenResponse(Request.Params["code"], Url.Action("GateWayResponse", "EmployerAccount", null, Request.Url.Scheme));
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
                var modelConfirm = new ConfirmNewPayeScheme(model)
                {
                    SelectedEntity = model.LegalEntities.SingleOrDefault(c => c.Id == selectedCompanyId)
                };

                return View("ConfirmLink", modelConfirm);
            }

        }

        [HttpGet]
        public ActionResult AddNewLegalEntity(ConfirmNewPayeScheme model)
        {
            model.SelectedEntity = new LegalEntity();
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

            var result = await _employerAccountPayeOrchestrator.GetCompanyDetails(new SelectEmployerModel { EmployerRef = model.SelectedEntity.CompanyNumber });

            var modelConfirm = new ConfirmNewPayeScheme(model)
            {
                SelectedEntity = new LegalEntity
                {
                    CompanyNumber = result.CompanyNumber,
                    DateOfIncorporation = result.DateOfIncorporation,
                    Name = result.CompanyName,
                    RegisteredAddress = result.RegisteredAddress
                }
            };

            return View("ConfirmLink", modelConfirm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Confirm(ConfirmNewPayeScheme model)
        {



            return RedirectToAction("Index", "EmployerAccountPaye", new { acccounId = model.AccountId });
        }

    }
}