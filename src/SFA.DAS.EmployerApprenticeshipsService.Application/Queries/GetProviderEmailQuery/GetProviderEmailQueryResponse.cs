using System.Collections.Generic;

namespace SFA.DAS.EAS.Application.Queries.GetProviderEmailQuery
{
    public class GetProviderEmailQueryResponse
    {
        public List<string> Emails { get; set; } = new List<string>();
    }
}