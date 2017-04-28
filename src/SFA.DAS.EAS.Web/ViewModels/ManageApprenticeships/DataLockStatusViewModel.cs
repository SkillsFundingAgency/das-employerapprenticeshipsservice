using System;

using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    public class DataLockStatusViewModel : ViewModelBase
    {
        public ITrainingProgramme CurrentProgram { get; set; }

        public ITrainingProgramme IlrProgram { get; set; }

        public DateTime? PeriodStartData { get; set; }

        public string HashedAccountId { get; set; }

        public string HashedApprenticeshipId { get; set; }
    }
}