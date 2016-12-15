using System.Collections.Generic;

using FluentValidation.Results;

using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Web.Models
{
    public sealed class ExtendedApprenticeshipViewModel
    {
        public ApprenticeshipViewModel Apprenticeship { get; set; }
        public List<ITrainingProgramme> ApprenticeshipProgrammes { get; set; }

        public ValidationResult ApprovalValidation { get; set; }
    }
    

    public sealed class ApprenticeshipProgramme
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}