CREATE PROCEDURE [employer_financial].[GetLevyFundsIn]
	@AccountId bigint
AS

SELECT CalendarPeriodYear, CalendarPeriodMonth, SUM([Amount]) AS FundsIn
FROM
(
	SELECT		(SELECT MONTH([TransactionDate])) AS CalendarPeriodMonth,
			(SELECT YEAR([TransactionDate])) AS CalendarPeriodYear,
			[Amount]
	FROM [employer_financial].[TransactionLine]
	WHERE AccountId = @AccountId
	AND TransactionType = /*Declaration*/ 1
) AS ungroupedQuery

GROUP BY CalendarPeriodYear, CalendarPeriodMonth
ORDER BY CalendarPeriodYear, CalendarPeriodMonth
