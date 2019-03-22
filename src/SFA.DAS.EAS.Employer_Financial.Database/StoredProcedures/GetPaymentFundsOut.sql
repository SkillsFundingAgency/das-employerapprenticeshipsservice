CREATE PROCEDURE [employer_financial].[GetPaymentFundsOut]
	@AccountId bigint
AS
SELECT	periodEnd.CalendarPeriodMonth AS CalendarPeriodMonth,
			periodEnd.CalendarPeriodYear AS CalendarPeriodYear,
			SUM(transactionLine.[Amount]) AS FundsOut
	FROM [employer_financial].[TransactionLine] transactionLine
	INNER JOIN [employer_financial].[PeriodEnd] periodEnd
	ON periodEnd.PeriodEndId = transactionLine.PeriodEnd
	
	WHERE transactionLine.AccountId = @AccountId

	GROUP BY CalendarPeriodYear, CalendarPeriodMonth
	ORDER BY CalendarPeriodYear, CalendarPeriodMonth