CREATE PROCEDURE [employer_financial].[GetTransactionSummary_ByAccountId]
	@AccountId BIGINT
AS
	SELECT 
		YEAR(DateCreated) As Year,
		MONTH(DateCreated) As Month,
		SUM(Amount) AS Amount
	FROM [employer_financial].[TransactionLine]
	WHERE AccountId = @AccountId
	GROUP BY YEAR(DateCreated), MONTH(DateCreated)