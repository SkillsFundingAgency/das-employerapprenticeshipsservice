using SFA.DAS.EmployerFinance.Queries.GetTransferTransactionDetails;
using SFA.DAS.EmployerFinance.Web.Extensions;
using System;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAccountTransactionsController : Controller
    {
        [Route("finance")]
        [Route("balance")]
        public ActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("finance"));
        }

        [Route("finance/downloadtransactions")]
        public ActionResult TransactionsDownload(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("finance/downloadtransactions"));
        }

        [Route("finance/{year}/{month}")]
        [Route("balance/{year}/{month}")]
        public ActionResult TransactionsView(string hashedAccountId, int year, int month)
        {
            return Redirect(Url.LegacyEasAccountAction($"finance/{year}/{month}"));
        }

        [Route("finance/levyDeclaration/details")]
        [Route("balance/levyDeclaration/details")]
        public ActionResult LevyDeclarationDetail(string hashedAccountId, DateTime fromDate, DateTime toDate)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"finance/levyDeclaration/details{paramString}"));
        }

        [Route("finance/provider/summary")]
        [Route("balance/provider/summary")]
        public ActionResult ProviderPaymentSummary(string hashedAccountId, long ukprn, DateTime fromDate, DateTime toDate)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"finance/provider/summary{paramString}"));
        }

        [Route("finance/course/standard/summary")]
        [Route("balance/course/standard/summary")]
        public ActionResult CourseStandardPaymentSummary(string hashedAccountId, long ukprn, string courseName,
            int? courseLevel, DateTime fromDate, DateTime toDate)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"finance/course/standard/summary{paramString}"));
        }

        [Route("finance/course/framework/summary")]
        [Route("balance/course/framework/summary")]
        public ActionResult CourseFrameworkPaymentSummary(string hashedAccountId, long ukprn, string courseName,
            int? courseLevel, int? pathwayCode, DateTime fromDate, DateTime toDate)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"finance/course/framework/summary{paramString}"));
        }

        [Route("finance/transfer/details")]
        [Route("balance/transfer/details")]
        public ActionResult TransferDetail(GetTransferTransactionDetailsQuery query)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"finance/transfer/details{paramString}"));
        }
    }
}