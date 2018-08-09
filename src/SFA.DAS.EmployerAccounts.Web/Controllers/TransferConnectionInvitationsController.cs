using SFA.DAS.EmployerAccounts.Queries.GetApprovedTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetLatestPendingReceivedTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetReceivedTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetRejectedTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Queries.SendTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}/transfers/connections/requests")]
    public class TransferConnectionInvitationsController : Controller
    {
        [Route]
        public ActionResult Index()
        {
            return Redirect(Url.LegacyEasAccountAction("transfers/connections/requests"));
        }

        [Route("start")]
        public ActionResult Start()
        {
            return Redirect(Url.LegacyEasAccountAction("transfers/connections/requests/start"));
        }

        [Route("send")]
        public ActionResult Send(SendTransferConnectionInvitationQuery query)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"transfers/connections/requests/send{paramString}"));
        }

        [Route("{transferConnectionInvitationId}/sent")]
        public ActionResult Sent(GetSentTransferConnectionInvitationQuery query, string transferConnectionInvitationId)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"transfers/connections/requests/{transferConnectionInvitationId}/sent{paramString}"));
        }

        [Route("{transferConnectionInvitationId}/receive")]
        public ActionResult Receive(GetReceivedTransferConnectionInvitationQuery query, string transferConnectionInvitationId)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"transfers/connections/requests/{transferConnectionInvitationId}/receive{paramString}"));
        }

        [Route("{transferConnectionInvitationId}/approved")]
        public ActionResult Approved(GetApprovedTransferConnectionInvitationQuery query, string transferConnectionInvitationId)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"transfers/connections/requests/{transferConnectionInvitationId}/approved{paramString}"));
        }

        [Route("{transferConnectionInvitationId}/rejected")]
        public ActionResult Rejected(GetRejectedTransferConnectionInvitationQuery query, string transferConnectionInvitationId)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"transfers/connections/requests/{transferConnectionInvitationId}/rejected{paramString}"));
        }

        [Route("{transferConnectionInvitationId}/details")]
        public ActionResult Details(GetTransferConnectionInvitationQuery query, string transferConnectionInvitationId)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"transfers/connections/requests/{transferConnectionInvitationId}/details{paramString}"));
        }

        [Route("{transferConnectionInvitationId}/deleted")]
        public ActionResult Deleted(string transferConnectionInvitationId)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"transfers/connections/requests/{transferConnectionInvitationId}/deleted{paramString}"));
        }

        [HttpGet]
        [Route("outstanding")]
        public ActionResult Outstanding(GetLatestPendingReceivedTransferConnectionInvitationQuery query)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"transfers/connections/requests/outstanding{paramString}"));
        }
    }
}