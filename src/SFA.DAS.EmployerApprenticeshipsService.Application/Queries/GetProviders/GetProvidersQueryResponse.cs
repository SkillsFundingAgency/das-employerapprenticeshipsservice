using System.Collections.Generic;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Queries.GetProviders
{
    public class GetProvidersQueryResponse
    {
        public List<Provider> Providers { get; set; }
    }
}