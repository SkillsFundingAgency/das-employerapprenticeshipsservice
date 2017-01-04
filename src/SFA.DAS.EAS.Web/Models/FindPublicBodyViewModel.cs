using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.EAS.Web.Models
{
    public class FindPublicBodyViewModel : FindOrganisationViewModel
    {
        public bool FoundMultiple { get; set; }
    }
}