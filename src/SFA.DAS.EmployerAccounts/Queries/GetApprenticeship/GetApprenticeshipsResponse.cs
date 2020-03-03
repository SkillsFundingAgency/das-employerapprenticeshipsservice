using SFA.DAS.EmployerAccounts.Models.Commitments;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship
{
    public class GetApprenticeshipsResponse
    {
        public IEnumerable<Apprenticeship> Apprenticeships { get; set; }
    }
}
