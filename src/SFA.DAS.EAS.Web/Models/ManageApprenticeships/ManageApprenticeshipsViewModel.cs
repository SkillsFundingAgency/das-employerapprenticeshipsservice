using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.Models.ManageApprenticeships
{
    public class ManageApprenticeshipsViewModel
    {
        // ToDo: use list view model
        public IEnumerable<ApprenticeshipDetailsViewModel> Apprenticeships { get; set; }

        public string HashedaccountId { get; set; }
    }
}