namespace SFA.DAS.EAS.Web.Validators.Messages
{
    public interface IApprenticeshipValidationErrorText
    {
        ValidationMessage CohortRef01 { get; }
        ValidationMessage CohortRef02 { get; }
        ValidationMessage DateOfBirth01 { get; }
        ValidationMessage DateOfBirth02 { get; }
        ValidationMessage EPAOrgID01 { get; }
        ValidationMessage EPAOrgID02 { get; }
        ValidationMessage FamilyName01 { get; }
        ValidationMessage FamilyName02 { get; }
        ValidationMessage FworkCode01 { get; }
        ValidationMessage FworkCode02 { get; }
        ValidationMessage FworkCode03 { get; }
        ValidationMessage GivenNames01 { get; }
        ValidationMessage GivenNames02 { get; }
        ValidationMessage LearnPlanEndDate01 { get; }
        ValidationMessage LearnPlanEndDate02 { get; }
        ValidationMessage LearnPlanEndDate03 { get; }
        ValidationMessage LearnStartDate01 { get; }
        ValidationMessage LearnStartDate02 { get; }
        ValidationMessage ProgType01 { get; }
        ValidationMessage ProgType02 { get; }
        ValidationMessage EmployerRef01 { get; }
        ValidationMessage PwayCode01 { get; }
        ValidationMessage PwayCode02 { get; }
        ValidationMessage PwayCode03 { get; }
        ValidationMessage StdCode01 { get; }
        ValidationMessage StdCode02 { get; }
        ValidationMessage StdCode03 { get; }
        ValidationMessage TrainingCode01 { get; }
        ValidationMessage TrainingPrice01 { get; }
        ValidationMessage TrainingPrice02 { get; }
        ValidationMessage Uln01 { get; }
        ValidationMessage Uln02 { get; }
    }
}