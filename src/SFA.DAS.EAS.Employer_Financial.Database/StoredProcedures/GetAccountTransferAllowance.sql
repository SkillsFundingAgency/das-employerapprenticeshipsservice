CREATE PROCEDURE [employer_financial].[GetAccountTransferAllowance]
	@accountId bigint = 0	
AS
	SELECT SUM(tl.Amount) FROM [employer_financial].[TransactionLine] tl
	INNER JOIN [employer_financial].[PeriodEnd] pe ON tl.PeriodEnd = pe.PeriodEndId
	WHERE pe.CalendarPeriodYear > Year(GETDATE()) - 2
	AND pe.CalendarPeriodMonth = 4
	AND tl.AccountId = @accountId

