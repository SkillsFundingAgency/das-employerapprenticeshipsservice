CREATE PROCEDURE [employer_financial].[GetPayeSchemesAddedByGovernmentGateway_ByAccountId]
	@accountId BIGINT
AS
	SELECT 
		* 
	FROM 
		[employer_financial].[AccountPaye] p
	WHERE
		p.AccountId = @accountId 
		AND p.Aorn IS NULL
