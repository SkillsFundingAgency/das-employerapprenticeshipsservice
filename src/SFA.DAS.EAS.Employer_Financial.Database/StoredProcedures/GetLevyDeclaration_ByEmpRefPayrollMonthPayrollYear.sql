CREATE PROCEDURE [employer_financial].[GetLevyDeclaration_ByEmpRefPayrollMonthPayrollYear]
	@EmpRef varchar(50),
	@PayrollYear varchar(10),
	@PayrollMonth int
AS
	select 
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
	where
		EmpRef = @EmpRef
	AND
		PayrollMonth = @PayrollMonth
	AND
		PayrollYear = @PayrollYear
	AND 
		EndOfYearAdjustment = 0
	AND 
		LastSubmission = 1
		
	

