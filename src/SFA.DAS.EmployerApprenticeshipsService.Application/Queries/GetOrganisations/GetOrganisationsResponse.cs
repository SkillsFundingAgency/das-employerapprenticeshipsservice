using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Application.Queries.GetOrganisations
{
    public class GetOrganisationsResponse
    {
        public PagedResponse<Organisation> Organisations { get; set; }
    }
}