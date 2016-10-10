using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public sealed class ExtendedApprenticeshipViewModel
    {
        public ApprenticeshipViewModel Apprenticeship { get; set; }
        public List<Standard> Standards { get; set; }
    }
}