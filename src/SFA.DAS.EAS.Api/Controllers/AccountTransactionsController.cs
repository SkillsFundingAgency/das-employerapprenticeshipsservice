using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Controllers;

[ApiController]
[Route("api/accounts/{hashedAccountId}/transactions")]
public class AccountTransactionsController : Microsoft.AspNetCore.Mvc.ControllerBase
{
    private readonly AccountTransactionsOrchestrator _orchestrator;

    public AccountTransactionsController(AccountTransactionsOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    [HttpGet(Name = "GetTransactionSummary")]
    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    public async Task<ActionResult> Index(string hashedAccountId)
    {
        var result = await _orchestrator.GetAccountTransactionSummary(hashedAccountId);

        if (result.Data == null)
        {
            return NotFound();
        }

        result.Data.ForEach(x => x.Href = Url.Link("GetTransactions", new { hashedAccountId, year = x.Year, month = x.Month }));

        return Ok(result.Data);
    }

    [Route("{year}/{month}", Name = "GetTransactions")]
    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet]
    public async Task<ActionResult<TransactionsViewModel>> GetTransactions(string hashedAccountId, int year = 0, int month = 0)
    {
        var result = await GetAccountTransactions(hashedAccountId, year, month);

        if (result.Data == null)
        {
            return NotFound();
        }

        return Ok(result.Data);
    }

    private async Task<OrchestratorResponse<TransactionsViewModel>> GetAccountTransactions(string hashedAccountId, int year, int month)
    {
        if (year == 0)
        {
            year = DateTime.Now.Year;
        }

        if (month == 0)
        {
            month = DateTime.Now.Month;
        }

        var result = await _orchestrator.GetAccountTransactions(hashedAccountId, year, month);
        return result;
    }
}