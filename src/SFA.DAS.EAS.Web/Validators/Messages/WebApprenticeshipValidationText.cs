﻿using System;

namespace SFA.DAS.EAS.Web.Validators.Messages
{
    public sealed class WebApprenticeshipValidationText : IApprenticeshipValidationErrorText
    {
        public ValidationMessage CohortRef01 { get { throw new NotImplementedException(); } }
        public ValidationMessage CohortRef02 { get { throw new NotImplementedException(); } }

        public ValidationMessage Uln01 =>
            new ValidationMessage("You must enter a 10-digit unique learner number", "ULN_01");
        public ValidationMessage Uln02 =>
            new ValidationMessage("The unique learner number of 9999999999 isn't valid", "ULN_02");

        public ValidationMessage FamilyName01 =>
            new ValidationMessage("Last name must be entered", "FamilyName_01");
        public ValidationMessage FamilyName02 =>
            new ValidationMessage("You must enter a last name that's no longer than 100 characters", "FamilyName_02");
        public ValidationMessage GivenNames01 =>
            new ValidationMessage("First name must be entered", "GivenNames_01");
        public ValidationMessage GivenNames02 =>
            new ValidationMessage("You must enter a first name that's no longer than 100 characters", "GivenNames_02");

        public ValidationMessage DateOfBirth01 =>
            new ValidationMessage("The Date of birth is not valid", "DateOfBirth_01");
        public ValidationMessage DateOfBirth02 =>
            new ValidationMessage("The apprentice must be at least 15 years old at the start of their training", "DateOfBirth_02");
        public ValidationMessage DateOfBirth06 =>
                    new ValidationMessage("Enter a valid year - the apprentice must be younger than 115 at the start of the current teaching year", "DateOfBirth_06");

        public ValidationMessage LearnStartDate01 =>
            new ValidationMessage("The start date is not valid", "LearnStartDate_01");
        public ValidationMessage LearnStartDate02 =>
            new ValidationMessage("The start date must not be earlier than May 2017", "LearnStartDate_02");

        public ValidationMessage LearnPlanEndDate01 =>
            new ValidationMessage("The end date is not valid", "LearnPlanEndDate_01");
        public ValidationMessage LearnPlanEndDate02 =>
            new ValidationMessage("The end date must not be on or before the start date", "LearnPlanEndDate_02");
        public ValidationMessage LearnPlanEndDate03 =>
            new ValidationMessage("The end date must not be in the past", "LearnPlanEndDate_03");
        public ValidationMessage LearnStartDate05 =>
           new ValidationMessage("The start date must be no later than one year after the end of the current teaching year", "LearnStartDate_05");

        public ValidationMessage TrainingPrice01 =>
            new ValidationMessage("Enter the total agreed training cost", "TrainingPrice_01");
        public ValidationMessage TrainingPrice02 =>
            new ValidationMessage("The total cost must be £100,000 or less", "TrainingPrice_02");

        public ValidationMessage EmployerRef01 =>
            new ValidationMessage("The Reference must be 20 characters or fewer", "EmployerRef_01");

        public ValidationMessage EPAOrgID01 { get { throw new NotImplementedException(); } }
        public ValidationMessage EPAOrgID02 { get { throw new NotImplementedException(); } }

        public ValidationMessage FworkCode01 { get { throw new NotImplementedException(); } }
        public ValidationMessage FworkCode02 { get { throw new NotImplementedException(); } }
        public ValidationMessage FworkCode03 { get { throw new NotImplementedException(); } }

        public ValidationMessage ProgType01 { get { throw new NotImplementedException(); } }
        public ValidationMessage ProgType02 { get { throw new NotImplementedException(); } }
        public ValidationMessage PwayCode01 { get { throw new NotImplementedException(); } }
        public ValidationMessage PwayCode02 { get { throw new NotImplementedException(); } }
        public ValidationMessage PwayCode03 { get { throw new NotImplementedException(); } }

        public ValidationMessage StdCode01 { get { throw new NotImplementedException(); } }
        public ValidationMessage StdCode02 { get { throw new NotImplementedException(); } }
        public ValidationMessage StdCode03 { get { throw new NotImplementedException(); } }

        public ValidationMessage TrainingCode01 =>
            new ValidationMessage("Training code cannot be empty", "DefaultErrorCode");
    }
}