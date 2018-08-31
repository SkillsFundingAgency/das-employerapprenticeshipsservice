using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class OrchestratorExtensions
    {
        public static async Task<string> GateWayUrlHelper(this EmployerVerificationOrchestratorBase orchestrator,
            string actionName, string controllerName, string urlScheme, UrlHelper urlHelper)
        {
            return await orchestrator.GetGatewayUrl(urlHelper.Action(actionName, controllerName, null, urlScheme));
        }

        public static void CreateOrganisationCookie(this OrganisationDetailsViewModel viewModel, IOrchestratorCookie orchestrator,
            HttpContextBase httpContext)
        {
            EmployerAccountData data;
            if (viewModel?.Name != null)
            {
                data = new EmployerAccountData
                {
                    OrganisationType = viewModel.Type,
                    OrganisationReferenceNumber = viewModel.ReferenceNumber,
                    OrganisationName = viewModel.Name,
                    OrganisationDateOfInception = viewModel.DateOfInception,
                    OrganisationRegisteredAddress = viewModel.Address,
                    OrganisationStatus = viewModel.Status ?? string.Empty,
                    PublicSectorDataSource = viewModel.PublicSectorDataSource,
                    Sector = viewModel.Sector,
                    NewSearch = viewModel.NewSearch
                };
            }
            else
            {
                var existingData = orchestrator.GetCookieData(httpContext);

                data = new EmployerAccountData
                {
                    OrganisationType = existingData.OrganisationType,
                    OrganisationReferenceNumber = existingData.OrganisationReferenceNumber,
                    OrganisationName = existingData.OrganisationName,
                    OrganisationDateOfInception = existingData.OrganisationDateOfInception,
                    OrganisationRegisteredAddress = existingData.OrganisationRegisteredAddress,
                    OrganisationStatus = existingData.OrganisationStatus,
                    PublicSectorDataSource = existingData.PublicSectorDataSource,
                    Sector = existingData.Sector,
                    NewSearch = existingData.NewSearch
                };
            }

            orchestrator.CreateCookieData(httpContext, data);
        }
    }
}