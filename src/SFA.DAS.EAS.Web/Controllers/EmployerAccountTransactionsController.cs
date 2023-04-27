using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

[DasAuthorize]
[Route("accounts/{HashedAccountId}")]
public class EmployerAccountTransactionsController : Controller
{
    public IConfiguration Configuration { get; set; }
    public EmployerAccountTransactionsController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }

    [Route("finance")]
    [Route("balance")]
    public IActionResult Index(string hashedAccountId)
    {
        return Redirect(Url.EmployerFinanceAction("finance", Configuration));
    }

    [Route("finance/employer-guidance")]
    public IActionResult EmployerGuidanceR02()
    {
        return Redirect(Url.EmployerFinanceAction("finance/employer-guidance", Configuration));
    }

    [Route("finance/downloadtransactions")]
    public IActionResult TransactionsDownload(string hashedAccountId)
    {
        return Redirect(Url.EmployerFinanceAction("finance/downloadtransactions", Configuration));
    }

    [Route("finance/{year}/{month}")]
    [Route("balance/{year}/{month}")]
    public IActionResult TransactionsView(string hashedAccountId, int year, int month)
    {
        return Redirect(Url.EmployerFinanceAction($"finance/{year}/{month}?{Request.QueryString}", Configuration));
    }

    [Route("finance/transfer/details")]
    [Route("balance/transfer/details")]
    public IActionResult TransferDetail()
    {
        return Redirect(Url.EmployerFinanceAction($"finance/transfer/details?{Request.QueryString}", Configuration));
    }

    [Route("finance/levyDeclaration/details")]
    [Route("balance/levyDeclaration/details")]
    public IActionResult LevyDeclarationDetail(string hashedAccountId, DateTime fromDate, DateTime toDate)
    {
        return Redirect(Url.EmployerFinanceAction($"finance/levyDeclaration/details?{Request.QueryString}", Configuration));
    }

    [Route("finance/provider/summary")]
    [Route("balance/provider/summary")]
    public IActionResult ProviderPaymentSummary(string hashedAccountId, long ukprn, DateTime fromDate, DateTime toDate)
    {
        return Redirect(Url.EmployerFinanceAction($"finance/provider/summary?{Request.QueryString}", Configuration));
    }

    [Route("finance/course/standard/summary")]
    [Route("balance/course/standard/summary")]
    public IActionResult CourseStandardPaymentSummary(string hashedAccountId, long ukprn, string courseName,
        int? courseLevel, DateTime fromDate, DateTime toDate)
    {
        return Redirect(Url.EmployerFinanceAction($"finance/course/standard/summary?{Request.QueryString}", Configuration));
    }

    [Route("finance/course/framework/summary")]
    [Route("balance/course/framework/summary")]
    public IActionResult CourseFrameworkPaymentSummary(string hashedAccountId, long ukprn, string courseName,
        int? courseLevel, int? pathwayCode, DateTime fromDate, DateTime toDate)
    {
        return Redirect(Url.EmployerFinanceAction($"finance/course/framework/summary?{Request.QueryString}", Configuration));
    }
}