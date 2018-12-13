using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Queries.GetPostcodeAddress
{
    public class GetPostcodeAddressResponse
    {
        public ICollection<Address> Addresses { get; set; }
    }
}
