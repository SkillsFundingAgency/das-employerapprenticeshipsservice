CREATE PROCEDURE [employer_financial].[GetLevyDeclarations_ByHashedAccountPayrollMonthPayrollYear]
	@accountId BIGINT,
	@payrollYear VARCHAR(10),
	@payrollMonth INT
AS
SELECT * FROM (
	SELECT
		x.*,
		(x.LevyDueYTD - isnull(LAG(x.LevyDueYTD) OVER(Partition by x.empref, x.payrollYear order by x.PayrollMonth), 0)) * x.TopUpPercentage as TopUp
	FROM 
		[employer_financial].[GetLevyDeclarations] x
	WHERE x.AccountId = @accountId AND x.LastSubmission = 1 AND x.PayrollYear = @payrollYear) y
WHERE y.PayrollMonth = @payrollMonth