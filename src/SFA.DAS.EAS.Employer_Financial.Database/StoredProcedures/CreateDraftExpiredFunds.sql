CREATE PROCEDURE [employer_financial].[CreateDraftExpiredFunds]
	@accountId BIGINT,
	@expiredFunds [employer_financial].[ExpiredFundsTable] READONLY,
	@now DATETIME
AS
	INSERT [employer_financial].[TransactionLine_EOF] (AccountId, DateCreated, TransactionDate, TransactionType, Amount)
	SELECT @accountId, @now, datefromparts(CalendarPeriodYear,CalendarPeriodMonth,1), /*ExpiredFund*/ 5, Amount
	FROM @expiredFunds
