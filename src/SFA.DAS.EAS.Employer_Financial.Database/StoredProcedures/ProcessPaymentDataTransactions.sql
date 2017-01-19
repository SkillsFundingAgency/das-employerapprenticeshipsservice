CREATE PROCEDURE [employer_financial].[ProcessPaymentDataTransactions]
	
AS


INSERT INTO [employer_financial].TransactionLine
select mainUpdate.* from
	(
	select 
			x.AccountId as AccountId,
			GETDATE() as DateCreated,
			null as submissionId,
			Max(pe.CompletionDateTime) as TransactionDate,
			3 as TransactionType,
			null as LevyDeclared,
			Sum(x.Amount) * -1 Amount,
			null as empref,
			PeriodEnd,
			UkPrn
		FROM 
			[employer_financial].[Payment] x
		inner join
			[employer_financial].[PeriodEnd] pe on pe.PeriodEndId = x.PeriodEnd
		where
			fundingsource = 1
		Group by
			UkPrn,PeriodEnd,accountId
	) mainUpdate
	inner join (
		select accountid,ukprn,periodend from [employer_financial].Payment where FundingSource = 1
	EXCEPT
		select accountid,ukprn,periodend from [employer_financial].transactionline where TransactionType = 3
	) dervx on dervx.accountId = mainUpdate.accountId and dervx.PeriodEnd = mainUpdate.PeriodEnd and dervx.Ukprn = mainUpdate.ukprn
