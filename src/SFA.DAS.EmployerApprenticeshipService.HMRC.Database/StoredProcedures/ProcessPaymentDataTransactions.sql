CREATE PROCEDURE [levy].[ProcessPaymentDataTransactions]
	
AS


INSERT INTO levy.TransactionLine
select mainUpdate.* from
	(
	select 
			x.AccountId as AccountId,
			Max(pe.CompletionDateTime) as CompletionDateTime,
			null as submissionId,
			DATETIMEFROMPARTS(MAX(pe.CalendarPeriodYear),max(pe.CalendarPeriodMonth),01,00,00,00,00) as TransactionDate,
			3 as TransactionType,
			Sum(x.Amount) * -1 Amount,
			null as empref,
			PeriodEnd,
			UkPrn
		FROM 
			[levy].[Payment] x
		inner join
			[levy].[PeriodEnd] pe on pe.PeriodEndId = x.PeriodEnd
		where
			fundingsource = 1
		Group by
			UkPrn,PeriodEnd,accountId
	) mainUpdate
	inner join (
		select accountid,ukprn,periodend from levy.Payment where FundingSource = 1
	EXCEPT
		select accountid,ukprn,periodend from levy.transactionline where TransactionType = 3
	) dervx on dervx.accountId = mainUpdate.accountId and dervx.PeriodEnd = mainUpdate.PeriodEnd and dervx.Ukprn = mainUpdate.ukprn
