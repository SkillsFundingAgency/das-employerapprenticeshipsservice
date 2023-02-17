using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Infrastructure.Models;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.Support.Shared.Authentication;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    [Authorize(Roles = "das-support-portal")]
    public class ChallengeController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IChallengeHandler _handler;
        private readonly ILog _log;

        public ChallengeController(IChallengeHandler handler, ILog log)
        {
            _handler = handler;
            _log = log;
        }

        [HttpGet]
        [Route("challenge/{id}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index(string id)
        {
            _log.Info($"ChallengeController-Index : Getting Response for id : {id}");

            var response = await _handler.Get(id);

            _log.Info($"ChallengeController-Index : Response : {response}");

            if (response.StatusCode != SearchResponseCodes.Success)
                return NotFound($"There was a problem finding the account {id}");

            return View(new ChallengeViewModel
            {
                Characters = response.Characters,
                Id = id
            });
        }

        [HttpPost]
        [Route("challenge/{id}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index(string id, ChallengeEntry challengeEntry)
        {
            var response = await _handler.Handle(Map(challengeEntry));

            if (response.IsValid)
            {
                return Json(new ChallengeValidationResult
                {
                    IsValidResponse = true
                });
            }


            var model = new ChallengeViewModel
            {
                Characters = response.Characters,
                Id = challengeEntry.Id,
                Url = challengeEntry.Url,
                HasError = true
            };

            return View(model);
        }

        private ChallengePermissionQuery Map(ChallengeEntry challengeEntry)
        {
            return new ChallengePermissionQuery
            {
                Id = challengeEntry.Id,
                Url = challengeEntry.Url,
                ChallengeElement1 = challengeEntry.Challenge1,
                ChallengeElement2 = challengeEntry.Challenge2,
                Balance = challengeEntry.Balance,
                FirstCharacterPosition = challengeEntry.FirstCharacterPosition,
                SecondCharacterPosition = challengeEntry.SecondCharacterPosition
            };
        }
    }
}