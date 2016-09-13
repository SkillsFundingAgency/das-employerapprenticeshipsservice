using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetProviders
{
    public class GetProvidersQueryResponse
    {
        public List<Provider> Providers { get; set; }
    }
}