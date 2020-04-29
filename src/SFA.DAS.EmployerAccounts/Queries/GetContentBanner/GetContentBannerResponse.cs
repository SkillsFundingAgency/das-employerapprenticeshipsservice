using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Queries.GetContentBanner
{
    public class GetContentBannerResponse
    {
        public MvcHtmlString ContentBanner { get; set; }
        public bool HasFailed { get; set; }
    }
}
