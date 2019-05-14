using System;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NServiceBus;
using System.Web.Mvc;
using SFA.DAS.HashingService;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("FundingJourney")]
    public class EmulatedFundingJourneyController : BaseController
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly EmployerTeamController _employerTeamController;
        private readonly IHashingService _hashingService;

        public EmulatedFundingJourneyController(IAuthenticationService owinWrapper) : base(owinWrapper)
        {

        }

        public EmulatedFundingJourneyController(
            IAuthenticationService owinWrapper, 
            IMultiVariantTestingService multiVariantTestingService, 
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            IEventPublisher eventPublisher,
            EmployerTeamController employerTeamController,
            IHashingService hashingService)
            : base (owinWrapper,multiVariantTestingService,flashMessage)
        {
            _eventPublisher = eventPublisher;
            _employerTeamController = employerTeamController;
            _hashingService = hashingService;
        }

        [HttpGet]
        [Route]
        public ActionResult Index(string HashedAccountId)
        {
            if (FeatureToggles.Features.EmulatedFundingJourney.Enabled)
            {
                return View(new EmulatedFundingViewModel {
                    Id = Guid.NewGuid(),
                    //todo: use SFA.DAS.Encoding
                    AccountId = _hashingService.DecodeValue(HashedAccountId),
                    HashedAccountId = HashedAccountId
                });
            }
            else
            {
                return View("NotFound");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route]
        public ActionResult SendEvent(EmulatedFundingViewModel model)
        {
            if (model.PublishEvent)
            {
                /*_eventPublisher.Publish(new ReservationCreatedEvent(
                    model.Id, model.AccountLegalEntityId, model.AccountLegalEntityName,
                    model.CourseId, model.StartDate, model.CourseName, model.EndDate,
                    DateTime.UtcNow, model.AccountId));*/
            }
            return _employerTeamController.ReturnFromEmulateFundingJourney(model);
        }
    }
}