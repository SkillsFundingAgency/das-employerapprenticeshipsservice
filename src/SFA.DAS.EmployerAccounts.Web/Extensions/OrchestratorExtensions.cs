using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class OrchestratorExtensions
    {
        public static async Task<string> GateWayUrlHelper(this EmployerVerificationOrchestratorBase orchestrator,
            string actionName, string controllerName, string urlScheme, UrlHelper urlHelper)
        {
            return await orchestrator.GetGatewayUrl(urlHelper.Action(actionName, controllerName, null, urlScheme));
        }
    }
}