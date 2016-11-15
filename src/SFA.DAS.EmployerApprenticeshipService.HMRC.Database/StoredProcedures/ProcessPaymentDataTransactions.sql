CREATE PROCEDURE [levy].[ProcessPaymentDataTransactions]
	
AS


INSERT INTO levy.TransactionLine
select mainUpdate.* from
	(
	select 
			x.AccountId,
			GetDate() as TransactionDate,
			null as submissionId,
			x.PaymentId as PaymentId,
			pe.CompletionDateTime,
			3 as TransactionType,
			x.Amount * -1 Amount,
			null as empref,
			PeriodEnd,
			UkPrn
		FROM 
			[levy].[Payment] x
		inner join
			[levy].[PeriodEnd] pe on pe.PeriodEndId = x.PeriodEnd
	) mainUpdate
	inner join (
		select PaymentId from levy.Payment
	EXCEPT
		select PaymentId from levy.Payment where TransactionType = 3
	) dervx on dervx.PaymentId = mainUpdate.PaymentId
