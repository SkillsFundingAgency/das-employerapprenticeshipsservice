CREATE PROCEDURE [employer_financial].[GetTotalSpendForLastYearByAccountId]
	@AccountId BIGINT
AS
	SELECT SUM(Amount) AS TotalSpend 
	FROM employer_financial.TransactionLine 
	WHERE TransactionDate >= DATEADD(DAY,DATEDIFF(DAY,-1,DATEADD(YEAR,-1,GETDATE())),0)
	AND AccountId = @AccountId
	AND TransactionType IN (3,4)
