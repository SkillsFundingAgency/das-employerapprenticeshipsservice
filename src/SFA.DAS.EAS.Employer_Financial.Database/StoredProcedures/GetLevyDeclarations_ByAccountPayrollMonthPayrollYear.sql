CREATE PROCEDURE [employer_financial].[GetLevyDeclarations_ByAccountPayrollMonthPayrollYear]
	@AccountId BIGINT,
	@PayrollYear VARCHAR(10),
	@PayrollMonth TINYINT
AS
SELECT * FROM [employer_financial].[GetLevyDeclarationAndTopUp] x
WHERE x.AccountId = @AccountId AND x.LastSubmission = 1 AND x.PayrollYear = @PayrollYear AND x.PayrollMonth = @PayrollMonth