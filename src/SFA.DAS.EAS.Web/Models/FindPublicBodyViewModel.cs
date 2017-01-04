using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Web.Models
{
    public class FindPublicBodyViewModel : FindOrganisationViewModel
    {
        public PagedResponse<PublicSectorOrganisation> Results { get; set; }
    }
}