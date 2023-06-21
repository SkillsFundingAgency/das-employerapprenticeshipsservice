using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Authorization;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[ApiController]
[Authorize(Policy = PolicyNames.IsSupportPortalUser)]
public class SearchController : ControllerBase
{
    private readonly IAccountHandler _handler;

    public SearchController(IAccountHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("api/search/accounts/{pagesize}/{pagenumber}")]
    public async Task<IActionResult> Accounts(int pageSize, int pageNumber)
    {
        var accounts = await _handler.FindAllAccounts(pageSize, pageNumber);
        return Ok(accounts);
    }

    [HttpGet("api/search/accounts/totalCount/{pageSize}")]
    public async Task<IActionResult> AllAccountsTotalCount(int pageSize)
    {
        var accounts = await _handler.TotalAccountRecords(pageSize);
        return Ok(accounts);
    }
}