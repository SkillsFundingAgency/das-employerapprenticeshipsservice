using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using StructureMap.Query;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class ApprenticeshipListItemGroupViewModel
    {
        public ITrainingProgramme TrainingProgramme { get; set; }

        public IList<ApprenticeshipListItemViewModel> Apprenticeships { get; set; }

        public string GroupId => TrainingProgramme == null ? "0" : TrainingProgramme.Id;

        public string GroupName => TrainingProgramme == null ? "No training course" : TrainingProgramme.Title;

        public int ApprenticeshipsOverFundingLimit
        {
            get
            {
                return TrainingProgramme == null ? 0 : Apprenticeships.Count(x => x.Cost > TrainingProgramme.MaxFunding);
            }
        }

        public bool ShowFundingLimitWarning
        {
            get
            {
                return TrainingProgramme != null && Apprenticeships.Any(x => x.Cost > TrainingProgramme.MaxFunding);
            }
        }
    }
}