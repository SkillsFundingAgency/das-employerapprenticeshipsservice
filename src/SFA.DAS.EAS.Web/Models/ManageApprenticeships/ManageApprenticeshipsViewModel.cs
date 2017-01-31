using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.Models.ManageApprenticeships
{
    public class ManageApprenticeshipsViewModel
    {
        public IEnumerable<ApprenticeshipDetailsViewModel> Apprenticeships { get; set; }

        public string HashedAccountId { get; set; }
    }
}