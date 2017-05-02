CREATE PROCEDURE [employer_financial].[GetPreviousTransactionsCount]	
	@accountId BIGINT,
	@fromDate datetime	
AS
select COUNT(*) FROM [employer_financial].[TransactionLine]
  WHERE AccountId = @accountId AND DateCreated < @fromDate
  

