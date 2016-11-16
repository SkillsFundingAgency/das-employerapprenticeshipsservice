CREATE PROCEDURE [levy].[GetTransactionLines_ByAccountId]
	@accountId BIGINT
AS
select 
	main.*
	,SUM(Amount) OVER(ORDER BY TransactionDate asc 
		RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW ) 
	     AS Balance
from
(
	SELECT 
	   [AccountId]
      ,[TransactionDate]
      ,MAX([TransactionType]) as TransactionType
      ,Sum(Amount) as Amount
	  ,UkPrn
  FROM [levy].[TransactionLine]
  WHERE AccountId = @accountId
  GROUP BY TransactionDate ,AccountId, UKPRN
  
) as main
order by TransactionDate desc
