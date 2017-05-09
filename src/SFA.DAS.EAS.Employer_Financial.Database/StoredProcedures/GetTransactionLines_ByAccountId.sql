CREATE PROCEDURE [employer_financial].[GetTransactionLines_ByAccountId]
	@accountId BIGINT,
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
	WHERE AccountId = @accountId and transactiontype in (1,2,3) group by accountid) as bal
left join
(
    SELECT 
       [AccountId]
      ,[TransactionType]
      ,Sum(Amount) as Amount
      ,UkPrn
      ,DateCreated
  FROM [employer_financial].[TransactionLine]
  WHERE AccountId = @accountId AND DateCreated >= @fromDate AND DateCreated <= @toDate
  GROUP BY DateCreated ,AccountId, UKPRN,[TransactionType]
  
) as main on main.AccountId = bal.AccountId
order by DateCreated desc, TransactionType desc, ukprn desc