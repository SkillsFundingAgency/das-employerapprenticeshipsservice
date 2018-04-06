CREATE PROCEDURE [employer_financial].[GetTransactionLines_ByAccountId]
	@AccountId BIGINT,
	@fromDate datetime,
	@toDate datetime
AS

select 
    main.*
    ,bal.balance AS Balance
from
(
	select Sum(amount) as balance,accountid 
	from employer_financial.TransactionLine 
	WHERE AccountId = @accountId and transactiontype in (1,2,3,4) group by accountid) as bal
left join
(
    SELECT 
       tl.[AccountId]
      ,tl.TransactionType
	  ,MAX(tl.TransactionDate) as TransactionDate
      ,Sum(tl.Amount) as Amount
      ,tl.Ukprn
      ,tl.DateCreated
	  ,tl.SfaCoInvestmentAmount
	  ,tl.EmployerCoInvestmentAmount
	  ,tl.PeriodEnd
	  ,ld.PayrollYear
	  ,ld.PayrollMonth
	  ,tl.TransferSenderAccountId as SenderAccountId
	  ,tl.TransferSenderAccountName as SenderAccountName
	  ,tl.TransferReceiverAccountId as ReceiverAccountId
	  ,tl.TransferReceiverAccountName as ReceiverAccountName	  
  FROM [employer_financial].[TransactionLine] tl
  LEFT JOIN [employer_financial].LevyDeclaration ld on ld.submissionid = tl.submissionid
  WHERE tl.AccountId = @accountId AND tl.DateCreated >= @fromDate AND DateCreated <= @toDate
  GROUP BY tl.DateCreated, tl.AccountId, tl.UKPRN, tl.SfaCoInvestmentAmount, tl.EmployerCoInvestmentAmount, 
  tl.TransactionType, ld.PayrollMonth, ld.PayrollYear, tl.TransferSenderAccountId, tl.TransferSenderAccountName,
  tl.TransferReceiverAccountId, tl.TransferReceiverAccountName
) as main on main.AccountId = bal.AccountId
order by DateCreated desc, TransactionType desc, ukprn desc