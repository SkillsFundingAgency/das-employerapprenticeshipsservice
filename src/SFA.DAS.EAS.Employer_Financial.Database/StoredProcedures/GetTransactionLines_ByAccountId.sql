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
	select Sum(Amount) as balance,AccountId 
	from employer_financial.TransactionLine 
	WHERE AccountId = @AccountId and TransactionType in (1,2,3) group by AccountId) as bal
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
	  ,ld.PayrollYear
	  ,ld.PayrollMonth
  FROM [employer_financial].[TransactionLine] tl
  LEFT JOIN [employer_financial].LevyDeclaration ld on ld.SubmissionId = tl.SubmissionId
  WHERE tl.AccountId = @AccountId AND tl.DateCreated >= @fromDate AND DateCreated <= @toDate
  GROUP BY tl.DateCreated, tl.AccountId, tl.Ukprn, tl.SfaCoInvestmentAmount, tl.EmployerCoInvestmentAmount, tl.TransactionType, ld.PayrollMonth, ld.PayrollYear
) as main on main.AccountId = bal.AccountId
order by DateCreated desc, TransactionType desc, Ukprn desc