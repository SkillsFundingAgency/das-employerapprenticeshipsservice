using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Employer;

namespace SFA.DAS.EAS.Application.Queries.GetPostcodeAddress
{
    public class GetPostcodeAddressResponse
    {
        public ICollection<Address> Addresses { get; set; }
    }
}
