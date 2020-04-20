using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship
{
    public class GetApprenticeshipsResponse
    {
        public IEnumerable<Apprenticeship> Apprenticeships { get; set; }
        public bool HasFailed { get; set; }
    }
}
