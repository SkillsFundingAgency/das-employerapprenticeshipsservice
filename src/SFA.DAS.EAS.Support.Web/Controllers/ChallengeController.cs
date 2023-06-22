using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Infrastructure.Models;
using SFA.DAS.EAS.Support.Web.Authorization;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[Authorize(Policy = PolicyNames.IsSupportPortalUser)]
public class ChallengeController : Controller
{
    private readonly IChallengeHandler _handler;
    private readonly ILogger<ChallengeController> _logger;

    public ChallengeController(IChallengeHandler handler, ILogger<ChallengeController> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    [HttpGet]
    [Route("challenge/{id}")]
    public async Task<IActionResult> Index(string id)
    {
        _logger.LogInformation("ChallengeController-Index : Getting Response for id : {id}", id);

        var response = await _handler.Get(id);

        _logger.LogInformation("ChallengeController-Index : Response : {response}", response);

        if (response.StatusCode != SearchResponseCodes.Success)
        {
            return NotFound($"There was a problem finding the account {id}");
        }

        return View(new ChallengeViewModel
        {
            Characters = response.Characters,
            Id = id
        });
    }

    [HttpPost]
    [Route("challenge/{id}")]
    public async Task<IActionResult> Index(string id, ChallengeEntry challengeEntry)
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

    private static ChallengePermissionQuery Map(ChallengeEntry challengeEntry)
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