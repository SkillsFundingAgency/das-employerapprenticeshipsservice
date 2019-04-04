using System.Web;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class OrchestratorExtensions
    {
        public static void CreateOrganisationCookie(this OrganisationDetailsViewModel viewModel,
            IOrchestratorCookie orchestrator,
            HttpContextBase httpContext)
        {
            EmployerAccountData data;
            if (viewModel?.Name != null)
            {
                data = new EmployerAccountData
                {
                    EmployerAccountOrganisationData = new EmployerAccountOrganisationData
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
                    }
                };
            }
            else
            {
                var existingData = orchestrator.GetCookieData(httpContext);

                data = new EmployerAccountData
                {
                    EmployerAccountOrganisationData = new EmployerAccountOrganisationData
                    {
                        OrganisationType = existingData.EmployerAccountOrganisationData.OrganisationType,
                        OrganisationReferenceNumber =
                            existingData.EmployerAccountOrganisationData.OrganisationReferenceNumber,
                        OrganisationName = existingData.EmployerAccountOrganisationData.OrganisationName,
                        OrganisationDateOfInception =
                            existingData.EmployerAccountOrganisationData.OrganisationDateOfInception,
                        OrganisationRegisteredAddress =
                            existingData.EmployerAccountOrganisationData.OrganisationRegisteredAddress,
                        OrganisationStatus = existingData.EmployerAccountOrganisationData.OrganisationStatus,
                        PublicSectorDataSource = existingData.EmployerAccountOrganisationData.PublicSectorDataSource,
                        Sector = existingData.EmployerAccountOrganisationData.Sector,
                        NewSearch = existingData.EmployerAccountOrganisationData.NewSearch
                    }
                };
            }

            orchestrator.CreateCookieData(httpContext, data);
        }
    }
}