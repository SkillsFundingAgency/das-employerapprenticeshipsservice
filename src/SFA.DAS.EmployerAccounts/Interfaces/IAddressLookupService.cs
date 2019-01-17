using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IAddressLookupService
    {
        Task<ICollection<Address>> GetAddressesByPostcode(string postcode);
    }
}
