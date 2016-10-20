﻿using System.Globalization;
using FluentValidation.Attributes;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EmployerApprenticeshipsService.Web.Validators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    [Validator(typeof(ApprenticeshipViewModelValidator))]
    public class ApprenticeshipViewModel
    {
        public string HashedId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string HashedAccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ApprenticeshipName => $"{FirstName} {LastName}";
        public string ULN { get; set; }
        public string TrainingId { get; set; } //standard or framework
        public string Cost { get; set; }
        public int? StartMonth { get; set; }

        public string StartMonthName
        {
            get
            {
                return StartMonth.HasValue ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(StartMonth.Value) : string.Empty;
            }
        }

        public int? StartYear { get; set; }
        public int? EndMonth { get; set; }

        public string EndMonthName
        {
            get
            {
                return EndMonth.HasValue ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(EndMonth.Value) : string.Empty;
            }
        }

        public int? EndYear { get; set; }
        public ApprenticeshipStatus Status { get; set; }
        public string AgreementStatus { get; set; }
        public bool CanBeApproved()
        {
            return Status == ApprenticeshipStatus.ReadyForApproval;
        }

        public bool CanBePaused()
        {
            return Status == ApprenticeshipStatus.Approved;
        }

        public bool CanBeResumed()
        {
            return Status == ApprenticeshipStatus.Paused;
        }
    }
}