CREATE PROCEDURE [employer_financial].[GetExpiredFunds]
	@AccountId bigint
AS

SELECT	YEAR(TransactionDate) AS CalendarPeriodYear,
		MONTH(TransactionDate) AS CalendarPeriodMonth,
		Amount
FROM	[employer_financial].[TransactionLine]
WHERE	AccountId = @AccountId
		AND TransactionType = /*ExpiredFund*/ 5
