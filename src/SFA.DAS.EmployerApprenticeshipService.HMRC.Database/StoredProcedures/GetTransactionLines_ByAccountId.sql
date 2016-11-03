CREATE PROCEDURE [levy].[GetTransactionLines_ByAccountId]
	@accountId BIGINT
AS
SELECT [AccountId]
      ,[SubmissionId]
      ,[PaymentId]
      ,[TransactionDate]
      ,[TransactionType]
      ,[Amount]
	  ,[EmpRef]
	  ,SUM(Amount) OVER(ORDER BY TransactionDate asc, TransactionType asc
		RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) 
         AS Balance
  FROM [levy].[TransactionLine]
  WHERE AccountId = @accountId
  order by TransactionDate desc, TransactionType desc