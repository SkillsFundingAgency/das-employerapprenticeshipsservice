using System.Threading.Tasks;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.ReferenceData.Types.DTO;
using Charity = SFA.DAS.EAS.Domain.Models.ReferenceData.Charity;
using CommonOrganisationType = SFA.DAS.Common.Domain.Types.OrganisationType;
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
            CommonOrganisationType? organisationType = null);

        Task<Organisation> GetLatestDetails(CommonOrganisationType organisationType, string identifier);

        /// <summary>
        ///     Returns true if the supplied organisation type can be retrieved by id.
        /// </summary>
        /// <param name="organisationType"></param>
        /// <returns>
        /// True if the supplied organisation supports fetching by ID otherwise false.
        /// </returns>
        /// <remarks>
        ///     Companies House and the Charity commission 
        /// </remarks>
        Task<bool> IsIdentifiableOrganisationType(CommonOrganisationType organisationType);
    }
}
