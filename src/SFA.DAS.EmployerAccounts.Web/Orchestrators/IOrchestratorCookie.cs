
namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public interface IOrchestratorCookie
{
    void CreateCookieData(HttpContext context, EmployerAccountData data);

    EmployerAccountData GetCookieData(HttpContext context);
}