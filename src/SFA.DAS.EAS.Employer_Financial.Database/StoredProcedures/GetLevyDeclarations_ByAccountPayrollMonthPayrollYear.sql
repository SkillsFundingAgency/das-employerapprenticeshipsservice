CREATE PROCEDURE [employer_financial].[GetLevyDeclarations_ByAccountPayrollMonthPayrollYear]
	@accountId BIGINT,
	@payrollYear VARCHAR(10),
	@payrollMonth TINYINT
AS
SELECT * FROM [employer_financial].[GetLevyDeclarationAndTopUp] x
WHERE x.AccountId = @accountId AND x.LastSubmission = 1 AND x.PayrollYear = @payrollYear AND x.PayrollMonth = @payrollMonth