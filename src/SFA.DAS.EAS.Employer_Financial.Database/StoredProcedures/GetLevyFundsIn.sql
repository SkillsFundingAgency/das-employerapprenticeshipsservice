CREATE PROCEDURE [employer_financial].[GetLevyFundsIn]
	@AccountId bigint
AS

SELECT CalendarPeriodYear, CalendarPeriodMonth, SUM([LevyDeclared]) AS FundsIn
FROM
(
	SELECT		(SELECT MONTH([TransactionDate])) AS CalendarPeriodMonth,
			(SELECT YEAR([TransactionDate])) AS CalendarPeriodYear,
			[LevyDeclared]
	FROM [employer_financial].[TransactionLine]
	WHERE transactionLine.AccountId = @AccountId
) AS ungroupedQuery

GROUP BY CalendarPeriodYear, CalendarPeriodMonth
ORDER BY CalendarPeriodYear, CalendarPeriodMonth
