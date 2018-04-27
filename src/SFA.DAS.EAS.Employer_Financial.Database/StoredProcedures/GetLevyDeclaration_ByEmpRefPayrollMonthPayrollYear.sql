CREATE PROCEDURE [employer_financial].[GetLevyDeclaration_ByEmpRefPayrollMonthPayrollYear]
	@EmpRef varchar(50),
	@PayrollYear varchar(10),
	@PayrollMonth int
AS
	select TOP 1
		*
	FROM 
		[employer_financial].[LevyDeclaration]
	where
		EmpRef = @EmpRef
	AND
		PayrollMonth = @PayrollMonth
	AND
		PayrollYear = @PayrollYear
	AND 
		EndOfYearAdjustment = 0
	ORDER BY 
		SubmissionDate DESC
		
	

