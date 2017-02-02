using System.Collections.Generic;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Queries.GetPostcodeAddress
{
    public class GetPostcodeAddressResponse
    {
        public ICollection<Address> Addresses { get; set; }
    }
}
