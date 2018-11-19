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
  WHERE tl.AccountId = @accountId 
		AND tl.DateCreated >= @fromDate 
		AND DateCreated <= @toDate
  GROUP BY 
		tl.DateCreated, 
		tl.AccountId, 
		tl.UKPRN, 
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
		DateCreated desc, 
		TransactionType desc, 
		ukprn desc