using SFA.DAS.EAS.Application.Queries.GetTransferTransactionDetails;
using SFA.DAS.EAS.Web.Extensions;
using System;
using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.Validation.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAccountTransactionsController: Controller
    {
        [Route("finance")]
        [Route("balance")]
        public ActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.EmployerFinanceAction("finance"));
        } 

        [ValidateMembership]
        [ImportModelStateFromTempData]
        [Route("finance/downloadtransactions")]
        public ActionResult TransactionsDownload(string hashedAccountId)
        {
            return Redirect(Url.EmployerFinanceAction("finance/downloadtransactions"));
        }

        [Route("finance/{year}/{month}")]
        [Route("balance/{year}/{month}")]
        public ActionResult TransactionsView(string hashedAccountId, int year, int month)
        {
            return Redirect(Url.EmployerFinanceAction($"finance/{year}/{month}?{Request.QueryString}"));
        }

        [Route("finance/transfer/details")]
        [Route("balance/transfer/details")]
        public ActionResult TransferDetail(GetTransferTransactionDetailsQuery query)
        {
            return Redirect(Url.EmployerFinanceAction($"finance/transfer/details{Request?.Url?.Query}"));
        }

        [Route("finance/levyDeclaration/details")]
        [Route("balance/levyDeclaration/details")]
        public ActionResult LevyDeclarationDetail(string hashedAccountId, DateTime fromDate, DateTime toDate)
        {
            return Redirect(Url.EmployerFinanceAction($"finance/levyDeclaration/details?{Request.QueryString}"));
        }

        [Route("finance/provider/summary")]
        [Route("balance/provider/summary")]
        public ActionResult ProviderPaymentSummary(string hashedAccountId, long ukprn, DateTime fromDate, DateTime toDate)
        {
            return Redirect(Url.EmployerFinanceAction($"finance/provider/summary?{Request.QueryString}"));
        }

        [Route("finance/course/standard/summary")]
        [Route("balance/course/standard/summary")]
        public ActionResult CourseStandardPaymentSummary(string hashedAccountId, long ukprn, string courseName,
            int? courseLevel, DateTime fromDate, DateTime toDate)
        {
            return Redirect(Url.EmployerFinanceAction($"finance/course/standard/summary?{Request.QueryString}"));
        }

        [Route("finance/course/framework/summary")]
        [Route("balance/course/framework/summary")]
        public ActionResult CourseFrameworkPaymentSummary(string hashedAccountId, long ukprn, string courseName,
            int? courseLevel, int? pathwayCode, DateTime fromDate, DateTime toDate)
        {
            return Redirect(Url.EmployerFinanceAction($"finance/course/framework/summary?{Request.QueryString}"));
        }
    }
}