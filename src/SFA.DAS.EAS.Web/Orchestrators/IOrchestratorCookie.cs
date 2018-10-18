using System.Web;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public interface IOrchestratorCookie
    {
        void CreateCookieData(HttpContextBase context, EmployerAccountData data);

        EmployerAccountData GetCookieData(HttpContextBase context);
    }
}
