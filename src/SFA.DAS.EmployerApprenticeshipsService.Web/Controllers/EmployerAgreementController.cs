using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    public class EmployerAgreementController : BaseController
    {
        private readonly IOwinWrapper _owinWrapper;
        private readonly IEmployerAgreementOrchestrator _orchestrator;

        public EmployerAgreementController(IOwinWrapper owinWrapper, IEmployerAgreementOrchestrator orchestrator)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));
            _owinWrapper = owinWrapper;
            _orchestrator = orchestrator;
        }

        [HttpGet]
        public async Task<ActionResult> Index(long accountId)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            var model = await _orchestrator.Get(accountId, userIdClaim);

            return View(model);
        }

        public ActionResult Add()
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            return View();
        }

        public async Task<ActionResult> View(long agreementid, long accountId)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            var agreement = await _orchestrator.GetById(agreementid, accountId, userIdClaim);

            return View(agreement);
        }
        
        [HttpPost]
        public async Task<ActionResult> Sign(long agreementid, long accountId, string understood, string legalEntityName)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            if (understood == "understood")
            {
                var response = await _orchestrator.SignAgreement(agreementid, accountId, userIdClaim);

                if (response.Status == HttpStatusCode.OK)
                {
                    TempData["successMessage"] = $"Agreement for {legalEntityName} has been signed";

                    return RedirectToAction("Index", new {accountId = accountId});
                }

                return View("DeadView", response);
            }

            TempData["successMessage"] = "You must indicate that you have read and understood the terms";

            return RedirectToAction("View", new { agreementId = agreementid, accountId = accountId });
        }
        
        [HttpPost]
        public async Task<ActionResult> FindLegalEntity(long accountId, string entityReferenceNumber)
        {
            var response = await _orchestrator.FindLegalEntity(accountId, entityReferenceNumber);
            
            return View(response.Data);
        }

        [HttpPost]
        public async Task<ActionResult> ViewEntityAgreement(
            long accountId, string name, string code, string address, DateTime incorporated)
        {
            var response = await _orchestrator.Create(accountId, name, code, address, incorporated);

            return View(response.Data);
        }

        [HttpPost]
        public async Task<ActionResult> CreateLegalEntity(long accountId, string name, 
            string code, string address, DateTime incorporated, bool signedAgreement)
        {
            var agreementId = await _orchestrator.AddLegalEntityToAccount(accountId, name, code, address, incorporated);

            if (!signedAgreement)
            {
                return RedirectToAction("Index", new { accountId });
            }

            //return await Sign(agreementId, accountId, "understood", name);

            return RedirectToAction("Sign", 
                new { agreementId = agreementId, accountId = accountId,
                    understood = "understood", legalEntityName = name });
        }
    }
}