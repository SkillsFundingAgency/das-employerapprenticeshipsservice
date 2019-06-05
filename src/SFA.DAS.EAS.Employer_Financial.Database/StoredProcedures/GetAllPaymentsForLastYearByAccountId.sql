CREATE PROCEDURE [employer_financial].[GetAllPaymentsForLastYearByAccountId]
	@AccountId BIGINT
AS
	SELECT SUM(Amount)*-1 AS TotalSpend 
	FROM employer_financial.TransactionLine 
	WHERE TransactionDate >= DATEADD(YEAR,-1,GETDATE())
	AND AccountId = @AccountId

