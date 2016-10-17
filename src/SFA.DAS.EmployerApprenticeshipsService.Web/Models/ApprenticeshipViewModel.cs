﻿using System.Globalization;
using FluentValidation;
using FluentValidation.Attributes;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public sealed class TestValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public TestValidator()
        {
            RuleFor(x => x.ULN).Matches("^$|^[1-9]{1}[0-9]{9}$").WithMessage("'ULN' is not in the correct format.");
            RuleFor(x => x.Cost).Matches("^$|^[1-9]{1}[0-9]*$");
        }
    }

    [Validator(typeof(TestValidator))]
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