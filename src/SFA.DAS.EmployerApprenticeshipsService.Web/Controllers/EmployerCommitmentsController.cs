using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class EmployerCommitmentsController : BaseController
    {
        private readonly EmployerCommitmentsOrchestrator _employerCommitmentsOrchestrator;

        public EmployerCommitmentsController(EmployerCommitmentsOrchestrator employerCommitmentsOrchestrator, IOwinWrapper owinWrapper, 
            IFeatureToggle featureToggle, IUserWhiteList userWhiteList) 
            : base(owinWrapper,featureToggle, userWhiteList)
        {
            if (employerCommitmentsOrchestrator == null)
                throw new ArgumentNullException(nameof(employerCommitmentsOrchestrator));
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            _employerCommitmentsOrchestrator = employerCommitmentsOrchestrator;
        }

        [HttpGet]
        public async Task<ActionResult> Index(long accountid)
        {
            var model = await _employerCommitmentsOrchestrator.GetAll(accountid);

            return View(model);
        }

        [HttpGet]
        public ActionResult Inform(long accountId)
        {
            var model = new CommitmentInformViewModel
            {
                AccountId = accountId
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> SelectLegalEntity(long accountId)
        {
            var model = await _employerCommitmentsOrchestrator.GetLegalEntities(accountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        public ActionResult SelectLegalEntity(CreateCommitmentModel commitment)
        {
            return RedirectToAction("SelectProvider", commitment);
        }

        [HttpGet]
        public async Task<ActionResult> SelectProvider(long accountId, long legalEntityId, string legalEntityName)
        {
            var model = await _employerCommitmentsOrchestrator.GetProviders(accountId, OwinWrapper.GetClaimValue(@"sub"));

            model.Data.Commitment.LegalEntityId = legalEntityId;
            model.Data.Commitment.LegalEntityName = legalEntityName;

            return View(model);
        }

        [HttpPost]
        public ActionResult SelectProvider(CreateCommitmentModel commitment)
        {
            return RedirectToAction("Create", commitment);
        }

        [HttpGet]
        public async Task<ActionResult> Create(CreateCommitmentModel commitment)
        {
            var model = await _employerCommitmentsOrchestrator.CreateSummary(commitment, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateCommitmentViewModel commitment)
        {
            await _employerCommitmentsOrchestrator.Create(commitment, OwinWrapper.GetClaimValue(@"sub"));

            return RedirectToAction("Index", new { accountId = commitment.AccountId });
        }

        [HttpGet]
        public async Task<ActionResult> Details(long accountId, long commitmentId)
        {
            var model = await _employerCommitmentsOrchestrator.Get(accountId, commitmentId);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(long accountId, long commitmentId, long apprenticeshipId)
        {
            var model = await _employerCommitmentsOrchestrator.GetApprenticeship(accountId, commitmentId, apprenticeshipId);

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(ApprenticeshipViewModel apprenticeship)
        {
            return RedirectToAction("Index", new { accountId = apprenticeship.AccountId, commitmentId = apprenticeship.CommitmentId });
        }

        [HttpGet]
        public async Task<ActionResult> Submit(long accountId, long commitmentId)
        {
            var commitment = await _employerCommitmentsOrchestrator.Get(accountId, commitmentId);
            
            var model = new SubmitCommitmentViewModel
            {
                SubmitCommitmentModel = new SubmitCommitmentModel
                {
                    AccountId = accountId,
                    CommitmentId = commitmentId
                },
                Commitment = commitment.Commitment
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Submit(SubmitCommitmentModel model)
        {
            await _employerCommitmentsOrchestrator.SubmitCommitment(model.AccountId, model.CommitmentId, model.Message);

            return RedirectToAction("Index", new { accountid = model.AccountId, commitmentId = model.CommitmentId });
        }

        [HttpPost]
        public async Task<ActionResult> ApproveApprenticeship(ApproveApprenticeshipModel model)
        {
            await _employerCommitmentsOrchestrator.ApproveApprenticeship(model);

            return RedirectToAction("Details", new { accountid = model.AccountId, commitmentId = model.CommitmentId });
        }

        [HttpGet]
        public async Task<ActionResult> PauseApprenticeship(long accountId, long commitmentId, long apprenticeshipId)
        {
            await _employerCommitmentsOrchestrator.PauseApprenticeship(accountId, commitmentId, apprenticeshipId);

            return RedirectToAction("Details", new { accountid = accountId, commitmentId = commitmentId });
        }

        [HttpGet]
        public async Task<ActionResult> ResumeApprenticeship(long accountId, long commitmentId, long apprenticeshipId)
        {
            await _employerCommitmentsOrchestrator.ResumeApprenticeship(accountId, commitmentId, apprenticeshipId);

            return RedirectToAction("Details", new { accountid = accountId, commitmentId = commitmentId });
        }

        [HttpGet]
        public async Task<ActionResult> CreateApprenticeship(long accountId, long commitmentId)
        {
            var model = await _employerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(accountId, commitmentId);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            await _employerCommitmentsOrchestrator.CreateApprenticeship(apprenticeship);

            return RedirectToAction("Details", new { accountId = apprenticeship.AccountId, commitmentId = apprenticeship.CommitmentId });
        }
    }
}