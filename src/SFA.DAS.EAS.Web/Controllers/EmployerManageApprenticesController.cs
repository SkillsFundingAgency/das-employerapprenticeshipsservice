using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;
using FluentValidation.Mvc;
using Microsoft.Azure;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.EAS.Web.Exceptions;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedAccountId}/apprentices/manage")]
    public class EmployerManageApprenticesController : BaseController
    {
        
        public EmployerManageApprenticesController(
            IOwinWrapper owinWrapper,
            IFeatureToggle featureToggle,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
                : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
        }

        [HttpGet]
        [Route("all")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> ListAll(string hashedAccountId, ApprenticeshipFiltersViewModel filtersViewModel)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/details", Name = "OnProgrammeApprenticeshipDetails")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange", Name = "ChangeStatusSelectOption")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> ChangeStatus(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }
        
        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/whentoapply", Name = "WhenToApplyChange")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> WhenToApplyChange(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusType changeType)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/confirm", Name = "StatusChangeConfirmation")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> StatusChangeConfirmation(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusType changeType, WhenToMakeChangeOptions whenToMakeChange, DateTime? dateOfChange)
        {
            return RedirectPermanentCommitmentsUrl();
        }
        
        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/edit", Name = "EditApprenticeship")]
        public async Task<ActionResult> Edit(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/confirm")]
        public async Task<ActionResult> ConfirmChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }
        
        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/view", Name = "ViewChanges")]
        public async Task<ActionResult> ViewChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        
        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/review", Name = "ReviewChanges")]
        public async Task<ActionResult> ReviewChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }
        
        [HttpGet]
        [Route("{hashedApprenticeshipId}/datalock/restart", Name = "RequestRestart")]
        public async Task<ActionResult> RequestRestart(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("paymentorder", Name = "PaymentOrder")]
        public async Task<ActionResult> PaymentOrder(string hashedAccountId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        
        private RedirectResult RedirectPermanentCommitmentsUrl()
        {
            var baseUrl = CloudConfigurationManager.GetSetting("EmployerCommitmentsBaseUrl").EndsWith("/")
                ? CloudConfigurationManager.GetSetting("EmployerCommitmentsBaseUrl")
                : CloudConfigurationManager.GetSetting("EmployerCommitmentsBaseUrl") + "/";

            var path = Request.Url.AbsolutePath;

            return RedirectPermanent($"{baseUrl}{path}");
        }
    }
}