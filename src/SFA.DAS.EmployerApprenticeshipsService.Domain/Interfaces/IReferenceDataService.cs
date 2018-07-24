using System.Threading.Tasks;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.ReferenceData.Types.DTO;
using Charity = SFA.DAS.EAS.Domain.Models.ReferenceData.Charity;
using OrganisationType = SFA.DAS.ReferenceData.Types.DTO.OrganisationType;
using PublicSectorOrganisation = SFA.DAS.EAS.Domain.Models.ReferenceData.PublicSectorOrganisation;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IReferenceDataService
    {
        Task<Charity> GetCharity(int registrationNumber);

        Task<PagedResponse<PublicSectorOrganisation>> SearchPublicSectorOrganisation(string searchTerm);

        Task<PagedResponse<PublicSectorOrganisation>> SearchPublicSectorOrganisation(
         string searchTerm,
         int pageNumber);

        Task<PagedResponse<PublicSectorOrganisation>> SearchPublicSectorOrganisation(
           string searchTerm,
           int pageNumber,
           int pageSize);

        Task<PagedResponse<OrganisationName>> SearchOrganisations(
            string searchTerm, 
            int pageNumber = 1, 
            int pageSize = 20, 
            Common.Domain.Types.OrganisationType? organisationType = null);

        Task<Organisation> GetLatestDetails(OrganisationType organisationType, string identifier);
    }
}
