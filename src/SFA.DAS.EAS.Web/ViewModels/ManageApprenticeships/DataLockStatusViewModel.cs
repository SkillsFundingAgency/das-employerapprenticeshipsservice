using System;
using System.Collections.Generic;

using FluentValidation.Attributes;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    [Validator(typeof(DataLockStatusViewModelViewModelValidator))]
    public class DataLockStatusViewModel : ViewModelBase
    {
        public ITrainingProgramme CurrentProgram { get; set; }

        public ITrainingProgramme IlrProgram { get; set; }

        public DateTime? PeriodStartData { get; set; }

        public string HashedAccountId { get; set; }

        public string HashedApprenticeshipId { get; set; }

        public string ProviderName { get; set; }

        public string LearnerName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string ChangesConfirmedError => GetErrorMessage(nameof(ChangesConfirmed));

        public bool? ChangesConfirmed { get; set; }

        public IList<PriceChange> PriceChanges { get; set; }
    }
}