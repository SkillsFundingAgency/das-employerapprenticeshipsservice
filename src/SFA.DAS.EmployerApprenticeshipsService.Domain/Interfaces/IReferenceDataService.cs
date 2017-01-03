using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.ReferenceData;

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

        

    }
}
