using System.Web;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public interface IOrchestratorCookie
    {
        void CreateCookieData(HttpContextBase context, EmployerAccountData data);

        EmployerAccountData GetCookieData(HttpContextBase context);
    }
}
