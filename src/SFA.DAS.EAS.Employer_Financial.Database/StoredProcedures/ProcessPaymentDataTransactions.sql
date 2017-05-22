CREATE PROCEDURE [employer_financial].[ProcessPaymentDataTransactions]
	
AS

--- Process Levy Payments ---
INSERT INTO [employer_financial].TransactionLine
select mainUpdate.* from
    (
    select 
            x.AccountId as AccountId,
            DATEFROMPARTS(DatePart(yyyy,GETDATE()),DatePart(MM,GETDATE()),DATEPART(dd,GETDATE())) as DateAdded,
            null as submissionId,
            Max(pe.CompletionDateTime) as TransactionDate,
            3 as TransactionType,
            null as LevyDeclared,
            Sum(ISNULL(p.Amount, 0)) * -1 Amount,
            null as empref,
            x.PeriodEnd,
            x.UkPrn,
            Sum(ISNULL(pco.Amount, 0)) * -1 as SfaCoInvestmentAmount,
            Sum(ISNULL(pci.Amount, 0)) * -1 as EmployerCoInvestmentAmount
        FROM 
            employer_financial.[Payment] x
		inner join [employer_financial].[PeriodEnd] pe 
				on pe.PeriodEndId = x.PeriodEnd
        left join [employer_financial].[Payment] p 
				on p.PeriodEnd = pe.PeriodEndId and p.PaymentId = x.PaymentId and p.FundingSource = 1
        left join [employer_financial].[payment] pco 
				on pco.PeriodEnd = pe.PeriodEndId and pco.PaymentId = x.PaymentId and pco.FundingSource = x.FundingSource and pco.FundingSource = 2 
        left join [employer_financial].[payment] pci 
				on pci.PeriodEnd = pe.PeriodEndId and pci.PaymentId = x.PaymentId  and pci.FundingSource = x.FundingSource and pci.FundingSource = 3 
        Group by
            x.UkPrn,x.PeriodEnd,x.accountId
    ) mainUpdate
    inner join (
        select accountid,ukprn,periodend from [employer_financial].Payment where FundingSource IN (1,2,3)      
    EXCEPT
        select accountid,ukprn,periodend from [employer_financial].transactionline where TransactionType = 3
    ) dervx on dervx.accountId = mainUpdate.accountId and dervx.PeriodEnd = mainUpdate.PeriodEnd and dervx.Ukprn = mainUpdate.ukprn
	

