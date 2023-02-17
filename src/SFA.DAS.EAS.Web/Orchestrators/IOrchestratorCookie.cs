using Microsoft.AspNetCore.Http;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public interface IOrchestratorCookie
    {
        void CreateCookieData(HttpContext context, EmployerAccountData data);

        EmployerAccountData GetCookieData(HttpContext context);
    }
}
