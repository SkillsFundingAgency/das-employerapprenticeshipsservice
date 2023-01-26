using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Support.ApplicationServices.Services;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    [Authorize(Roles = "das-support-portal")]
    public class SearchController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly IAccountHandler _handler;

        public SearchController(IAccountHandler handler)
        {
            _handler = handler;
        }

        [HttpGet]
        [Route("api/search/accounts/{pagesize}/{pagenumber}")]
        public async Task<IHttpActionResult> Accounts(int pageSize, int pageNumber)
        {
            var accounts = await _handler.FindAllAccounts(pageSize, pageNumber);
            return Json(accounts);
        }

        [HttpGet]
        [Route("api/search/accounts/totalCount/{pageSize}")]
        public async Task<IHttpActionResult> AllAccountsTotalCount(int pageSize)
        {
            var accounts = await _handler.TotalAccountRecords(pageSize);
            return Json(accounts);
        }
    }
}