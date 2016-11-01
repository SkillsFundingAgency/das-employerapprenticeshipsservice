using System.Collections.Generic;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Queries.GetAccountPayeSchemes
{
    public class GetAccountPayeSchemesResponse
    {
        public List<PayeView> PayeSchemes { get; set; }
    }
}