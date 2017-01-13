CREATE PROCEDURE [employer_transactions].[ProcessPaymentDataTransactions]
	
AS


INSERT INTO [employer_transactions].TransactionLine
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
			[employer_transactions].[Payment] x
		inner join
			[employer_transactions].[PeriodEnd] pe on pe.PeriodEndId = x.PeriodEnd
		where
			fundingsource = 1
		Group by
			UkPrn,PeriodEnd,accountId
	) mainUpdate
	inner join (
		select accountid,ukprn,periodend from [employer_transactions].Payment where FundingSource = 1
	EXCEPT
		select accountid,ukprn,periodend from [employer_transactions].transactionline where TransactionType = 3
	) dervx on dervx.accountId = mainUpdate.accountId and dervx.PeriodEnd = mainUpdate.PeriodEnd and dervx.Ukprn = mainUpdate.ukprn
