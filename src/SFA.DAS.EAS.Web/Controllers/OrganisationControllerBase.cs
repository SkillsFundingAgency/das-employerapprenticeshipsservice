using System.Net;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class OrganisationControllerBase : BaseController
    {
        private readonly OrganisationOrchestratorBase _orchestrator;

        public OrganisationControllerBase(OrganisationOrchestratorBase orchestrator, IOwinWrapper owinWrapper, IFeatureToggle featureToggle, IUserWhiteList userWhiteList) : base(owinWrapper, featureToggle, userWhiteList)
        {
            _orchestrator = orchestrator;
        }

        private async Task<OrchestratorResponse<PublicSectorOrganisationSearchResultsViewModel>> FindPublicSectorOrganisation(string publicSectorOrganisationName, string hashedAccountId, string userIdClaim)
        {
            var response = await _orchestrator.FindPublicSectorOrganisation(publicSectorOrganisationName);

            switch (response.Status)
            {
                case HttpStatusCode.NotFound:
                    TempData["publicBodyError"] = "No public organsiations were not found using your search term";
                    break;
            }

            return response;
        }

        private async Task<OrchestratorResponse<OrganisationDetailsViewModel>> FindCompany(string companiesHouseNumber)
        {
            var response = await _orchestrator.GetCompany(companiesHouseNumber);

            switch (response.Status)
            {
                case HttpStatusCode.NotFound:
                    TempData["companyError"] = "Company not found";
                    break;
                case HttpStatusCode.Conflict:
                    TempData["companyError"] = "Enter a company that isn't already registered";
                    break;
            }

            return response;
        }

        private async Task<OrchestratorResponse<OrganisationDetailsViewModel>> FindCharity(string charityRegNo)
        {
            var response = await _orchestrator.GetCharity(charityRegNo);

            switch (response.Status)
            {
                case HttpStatusCode.NotFound:
                    TempData["charityError"] = "Charity not found";
                    break;
                case HttpStatusCode.BadRequest:
                    TempData["charityError"] = "Charity is removed";
                    break;
                case HttpStatusCode.Conflict:
                    TempData["charityError"] = "Charity is already added";
                    break;
            }

            return response;
        }
    }
}