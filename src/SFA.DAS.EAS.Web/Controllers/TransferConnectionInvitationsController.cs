using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EAS.Application.Queries.GetApprovedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetLatestPendingReceivedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetReceivedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetRejectedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.Validation.Mvc;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [Feature(FeatureType.TransferConnectionRequests)]
    [ValidateMembership]
    [RoutePrefix("accounts/{HashedAccountId}/transfers/connections/requests")]
    public class TransferConnectionInvitationsController : Controller
    {

        [Route]
        public ActionResult Index()
        {
            return Redirect(Url.EmployerFinanceAction($"transfers/connections/requests{Request.QueryString}"));
        }

        [ImportModelStateFromTempData]
        [Route("start")]
        public ActionResult Start()
        {
            return Redirect(Url.EmployerFinanceAction($"transfers/connections/requests/start{Request.QueryString}"));
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("send")]
        public ActionResult Send(SendTransferConnectionInvitationQuery query)
        {
            return Redirect(Url.EmployerFinanceAction($"transfers/connections/requests/send{Request.QueryString}"));
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/sent")]
        public ActionResult Sent(GetSentTransferConnectionInvitationQuery query)
        {
            return Redirect(Url.EmployerFinanceAction($"transfers/connections/requests/{query.TransferConnectionInvitationId}/sent{Request.QueryString}"));
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/receive")]
        public ActionResult Receive(GetReceivedTransferConnectionInvitationQuery query)
        {
            return Redirect(Url.EmployerFinanceAction($"transfers/connections/requests/{query.TransferConnectionInvitationId}/receive{Request.QueryString}"));
        }


        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/approved")]
        public ActionResult Approved(GetApprovedTransferConnectionInvitationQuery query)
        {
            return Redirect(Url.EmployerFinanceAction($"transfers/connections/requests/{query.TransferConnectionInvitationId}/approved{Request.QueryString}"));
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/rejected")]
        public ActionResult Rejected(GetRejectedTransferConnectionInvitationQuery query)
        {
            return Redirect(Url.EmployerFinanceAction($"transfers/connections/requests/{query.TransferConnectionInvitationId}/rejected{Request.QueryString}"));
        }

        [HttpNotFoundForNullModel]
        [Route("{transferConnectionInvitationId}/details")]
        public ActionResult Details(GetTransferConnectionInvitationQuery query)
        {
            return Redirect(Url.EmployerFinanceAction($"transfers/connections/requests/{query.TransferConnectionInvitationId}/details{Request.QueryString}"));
        }

        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/deleted")]
        public ActionResult Deleted(string transferConnectionInvitationId)
        {
            return Redirect(Url.EmployerFinanceAction($"transfers/connections/requests/{transferConnectionInvitationId}/deleted{Request.QueryString}"));
        }

        [HttpGet]
        [Route("outstanding")]
        public ActionResult Outstanding(GetLatestPendingReceivedTransferConnectionInvitationQuery query)
        {
            return Redirect(Url.EmployerFinanceAction($"transfers/connections/requests/outstanding{Request.QueryString}"));
        }
    }
}