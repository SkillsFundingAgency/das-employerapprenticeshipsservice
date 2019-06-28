CREATE PROCEDURE [employer_financial].[GetTransactionLines_ByAccountId]
    @AccountId BIGINT,
    @fromDate DATETIME,
    @toDate DATETIME
AS

    SELECT 
		  tl.[AccountId],
		  tl.TransactionType,
		  MAX(tl.TransactionDate) as TransactionDate,
		  Sum(tl.Amount) as Amount,
		  tl.Ukprn,
		  providerName.ProviderName,
		  tl.DateCreated,
		  tl.SfaCoInvestmentAmount,
		  tl.EmployerCoInvestmentAmount,
		  tl.PeriodEnd,
		  ld.PayrollYear,
		  ld.PayrollMonth,
		  tl.TransferSenderAccountId as SenderAccountId,
		  tl.TransferSenderAccountName as SenderAccountName,
		  tl.TransferReceiverAccountId as ReceiverAccountId,
		  tl.TransferReceiverAccountName as ReceiverAccountName
  FROM	[employer_financial].[TransactionLine] tl
		LEFT JOIN [employer_financial].LevyDeclaration ld 
			on ld.submissionid = tl.submissionid
		LEFT JOIN 
		(SELECT TOP(1) pmd.ProviderName,
		p.Ukprn
		FROM [employer_financial].Payment p
		LEFT JOIN [employer_financial].PaymentMetaData pmd
			on pmd.Id = p.PaymentMetaDataId) As providerName
		ON providerName.Ukprn = tl.Ukprn

  WHERE tl.AccountId = @accountId 
		AND tl.DateCreated >= @fromDate 
		AND DateCreated <= @toDate
  GROUP BY 
		tl.DateCreated, 
		tl.AccountId, 
		tl.UKPRN,
		providerName.ProviderName, 
		tl.SfaCoInvestmentAmount, 
		tl.EmployerCoInvestmentAmount, 
		tl.TransactionType, 
		tl.PeriodEnd, 
		ld.PayrollMonth, 
		ld.PayrollYear, 
		tl.TransferSenderAccountId, 
		tl.TransferSenderAccountName, 
		tl.TransferReceiverAccountId, 
		tl.TransferReceiverAccountName
order by 
		tl.DateCreated desc, 
		tl.TransactionType desc, 
		tl.ukprn desc