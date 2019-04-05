CREATE PROCEDURE [employer_financial].[ProcessPaymentDataTransactions]
	@AccountId BIGINT
AS

--- Process Levy Payments ---
INSERT INTO [employer_financial].[TransactionLine]
select mainUpdate.* from
    (
    select 
            x.AccountId as AccountId,
            DATEFROMPARTS(DatePart(yyyy,GETDATE()),DatePart(MM,GETDATE()),DATEPART(dd,GETDATE())) as DateAdded,
            null as SubmissionId,
            Max(pe.CompletionDateTime) as TransactionDate,
            3 as TransactionType,			
            null as LevyDeclared,
            Sum(ISNULL(p.Amount, 0)) * -1 Amount,
            null as EmpRef,
            x.PeriodEnd,
            x.Ukprn,
            Sum(ISNULL(pco.Amount, 0)) * -1 as SfaCoInvestmentAmount,
            Sum(ISNULL(pci.Amount, 0)) * -1 as EmployerCoInvestmentAmount,
			0 as EnglishFraction,
			null as TransferSenderAccountId,
			null as TransferSenderAccountName,
			null as TransferReceiverAccountId,
			null as TransferReceiverAccountName		
        FROM 
            employer_financial.[Payment] x
		inner join [employer_financial].[PeriodEnd] pe 
				on pe.PeriodEndId = x.PeriodEnd
        left join [employer_financial].[Payment] p 
				on p.PeriodEnd = pe.PeriodEndId and p.PaymentId = x.PaymentId and p.FundingSource IN (1, 5)
        left join [employer_financial].[Payment] pco 
				on pco.PeriodEnd = pe.PeriodEndId and pco.PaymentId = x.PaymentId and pco.FundingSource = x.FundingSource and pco.FundingSource = 2 
        left join [employer_financial].[Payment] pci 
				on pci.PeriodEnd = pe.PeriodEndId and pci.PaymentId = x.PaymentId  and pci.FundingSource = x.FundingSource and pci.FundingSource = 3 
		WHERE x.AccountId = @AccountId
        Group by
            x.Ukprn,x.PeriodEnd,x.AccountId
    ) mainUpdate
    inner join (
        select AccountId,Ukprn,PeriodEnd from [employer_financial].Payment where FundingSource IN (1,2,3,5)      
    EXCEPT
        select AccountId,Ukprn,PeriodEnd from [employer_financial].[TransactionLine] where TransactionType = 3
    ) dervx on dervx.AccountId = mainUpdate.AccountId and dervx.PeriodEnd = mainUpdate.PeriodEnd and dervx.Ukprn = mainUpdate.Ukprn
	

