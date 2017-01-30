using System.Collections.Generic;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EAS.Web.Models
{
    public sealed class ExtendedApprenticeshipViewModel
    {
        public ApprenticeshipViewModel Apprenticeship { get; set; }
        public List<ITrainingProgramme> ApprenticeshipProgrammes { get; set; }
    }

    public sealed class ApprenticeshipProgramme
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}