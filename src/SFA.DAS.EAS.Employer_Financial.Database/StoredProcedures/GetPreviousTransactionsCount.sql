CREATE PROCEDURE [employer_financial].[GetPreviousTransactionsCount]	
	@AccountId BIGINT,
	@fromDate datetime	
AS
select COUNT(*) FROM [employer_financial].[TransactionLine]
  WHERE AccountId = @AccountId AND DateCreated < @fromDate
  

