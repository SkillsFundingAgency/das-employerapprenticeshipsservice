CREATE PROCEDURE [employer_financial].[GetTransactionSummary_ByAccountId]
	@AccountId BIGINT
AS
	SELECT 
		YEAR(DateCreated),
		MONTH(DateCreated),
		SUM(Amount) AS Total
	FROM [employer_financial].[TransactionLine]
	WHERE AccountId = @AccountId
	GROUP BY YEAR(DateCreated), MONTH(DateCreated)