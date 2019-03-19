CREATE PROCEDURE [employer_financial].[CreateExpiredFunds]
	@accountId BIGINT NOT NULL,
	@expiredFunds [employer_financial].[ExpiredFundsTable] READONLY
AS
	INSERT [employer_financial].[TransactionLine] (AccountId, DateCreated, TransactionDate, TransactionType, Amount)
	SELECT @accountId, GETDATE(), datefromparts(CalendarPeriodYear,CalendarPeriodMonth,0), /*ExpiredFund*/ 5, Amount
	FROM @expiredFunds
