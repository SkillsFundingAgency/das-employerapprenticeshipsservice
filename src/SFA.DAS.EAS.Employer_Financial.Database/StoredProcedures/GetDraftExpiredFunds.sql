CREATE PROCEDURE [employer_financial].[GetDraftExpiredFunds]
	@AccountId bigint
AS

SELECT	YEAR(TransactionDate) AS CalendarPeriodYear,
		MONTH(TransactionDate) AS CalendarPeriodMonth,
		Amount
FROM	[employer_financial].[TransactionLine_EOF]
WHERE	AccountId = @AccountId
		AND TransactionType = /*ExpiredFund*/ 5
