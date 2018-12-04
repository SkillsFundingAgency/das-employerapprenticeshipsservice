CREATE PROCEDURE [employer_financial].[GetEffectivePeriod12Declaration]
	@EmpRef varchar(50),
	@PayrollYear varchar(10),
	@YearEndAdjustmentCutOff DateTime
AS
	SELECT TOP 1
		Id, 
		AccountId, 
		EmpRef, 
		LevyDueYTD, 
		LevyAllowanceForYear, 
		SubmissionDate, 
		SubmissionId, 
		PayrollYear, 
		PayrollMonth, 
		CreatedDate, 
		EndOfYearAdjustment,
		EndOfYearAdjustmentAmount,
		LevyAllowanceForYear,
		DateCeased,
		InactiveFrom,
		InactiveTo,
		HmrcSubmissionId,
		NoPaymentForPeriod
	FROM 
		[employer_financial].[GetLevyDeclaration]
	WHERE	EmpRef = @EmpRef
			AND	PayrollYear = @PayrollYear
			AND (LastSubmission = 1 OR (EndOfYearAdjustment = 1 AND SubmissionDate < @yearEndAdjustmentCutOff))
	ORDER BY SubmissionDate DESC		
	

