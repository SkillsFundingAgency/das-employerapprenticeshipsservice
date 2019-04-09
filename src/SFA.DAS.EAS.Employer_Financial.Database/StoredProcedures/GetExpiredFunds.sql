CREATE PROCEDURE [employer_financial].[GetExpiredFunds]
	@AccountId bigint
AS

SELECT	YEAR(DateCreated) AS CalendarPeriodYear,
		MONTH(DateCreated) AS CalendarPeriodMonth,
		Amount
FROM	[employer_financial].[TransactionLine]
WHERE	AccountId = @AccountId
		AND TransactionType = /*ExpiredFund*/ 5
