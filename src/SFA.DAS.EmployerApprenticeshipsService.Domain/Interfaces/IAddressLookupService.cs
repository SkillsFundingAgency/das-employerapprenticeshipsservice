using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Employer;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IAddressLookupService
    {
        Task<ICollection<Address>> GetAddressesByPostcode(string postcode);
    }
}
