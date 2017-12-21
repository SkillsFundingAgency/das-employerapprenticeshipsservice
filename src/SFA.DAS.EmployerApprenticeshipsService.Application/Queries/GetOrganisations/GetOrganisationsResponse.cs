using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Application.Queries.GetOrganisations
{
    public class GetOrganisationsResponse
    {
        public PagedResponse<OrganisationName> Organisations { get; set; }
    }
}