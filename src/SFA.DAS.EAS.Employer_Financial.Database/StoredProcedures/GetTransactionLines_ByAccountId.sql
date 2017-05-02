CREATE PROCEDURE [employer_financial].[GetTransactionLines_ByAccountId]
	@accountId BIGINT,
	@fromDate datetime,
	@toDate datetime
AS
select 
    main.*
    ,SUM(Amount) OVER(ORDER BY DateCreated,Transactiontype,ukprn asc
        RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW ) 
         AS Balance
from
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
  
) as main
order by DateCreated desc, TransactionType desc, ukprn desc