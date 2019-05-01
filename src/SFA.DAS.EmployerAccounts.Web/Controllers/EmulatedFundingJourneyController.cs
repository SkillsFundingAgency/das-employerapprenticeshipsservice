using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NServiceBus;
using System.Web.Mvc;
using SFA.DAS.EmployerAccounts.Messages.Events;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("FundingJourney")]
    public class EmulatedFundingJourneyController : BaseController
    {
        private readonly IEventPublisher _eventPublisher;
        private EmployerTeamController _employerTeamController;

        public EmulatedFundingJourneyController(IAuthenticationService owinWrapper) : base(owinWrapper)
        {

        }

        public EmulatedFundingJourneyController(
            IAuthenticationService owinWrapper, 
            IMultiVariantTestingService multiVariantTestingService, 
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            IEventPublisher eventPublisher,
            EmployerTeamController employerTeamController)
            : base (owinWrapper,multiVariantTestingService,flashMessage)
        {
            _eventPublisher = eventPublisher;
            _employerTeamController = employerTeamController;
        }

        [HttpGet]
        [Route]
        public ActionResult Index(string HashedAccountId)
        {

            return View(new EmulatedFundingViewModel { HashedAccountId=HashedAccountId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route]
        public ActionResult SendEvent(EmulatedFundingViewModel model)
        {
            if (model.PublishEvent)
            {
                _eventPublisher.Publish(new ReserveFundingCreatedEvent
                {
                    HashedAccountId = model.HashedAccountId,
                    CourseCode = model.CourseCode,
                    LegalEntityId = model.LegalEntityId,
                    CourseName = model.CourseName,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ReservationId = model.ReservationId
                });
            }
            return _employerTeamController.ReturnFromEmulateFundingJourney(model);
        }
    }
}