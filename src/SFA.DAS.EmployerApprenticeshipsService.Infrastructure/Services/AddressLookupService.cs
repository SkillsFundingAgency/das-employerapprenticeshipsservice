using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class AddressLookupService : IAddressLookupService
    {
        public async Task<ICollection<Address>> GetAddressesByPostcode(string postcode)
        {
            throw new NotImplementedException();
        }
    }
}
