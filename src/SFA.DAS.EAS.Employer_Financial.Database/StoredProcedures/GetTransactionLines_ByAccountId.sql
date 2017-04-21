CREATE PROCEDURE [employer_financial].[GetTransactionLines_ByAccountId]
	@accountId BIGINT
AS
select 
	main.*
	,SUM(Amount) OVER(ORDER BY DateCreated asc 
		RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW ) 
	     AS Balance
from
(
	SELECT 
	   [AccountId]
      ,Max([TransactionDate]) as TransactionDate
      ,TransactionType
      ,Sum(Amount) as Amount
	  ,UkPrn
	  ,DateCreated
  FROM [employer_financial].[TransactionLine]
  WHERE AccountId = @accountId
  GROUP BY DateCreated ,AccountId, UKPRN, TransactionType
  
) as main
order by TransactionDate desc
