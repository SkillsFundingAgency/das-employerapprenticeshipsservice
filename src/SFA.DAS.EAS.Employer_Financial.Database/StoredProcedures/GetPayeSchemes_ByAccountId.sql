CREATE PROCEDURE [employer_financial].[GetPayeSchemes_ByAccountId]
	@accountId BIGINT
AS
	SELECT 
		* 
	FROM 
		[employer_financial].[AccountPaye] p
	WHERE
		p.AccountId = @accountId
