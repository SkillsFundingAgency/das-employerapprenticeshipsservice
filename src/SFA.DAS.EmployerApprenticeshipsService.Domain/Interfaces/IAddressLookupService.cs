using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IAddressLookupService
    {
        Task<ICollection<Address>> GetAddressesByPostcode(string postcode);
    }
}
