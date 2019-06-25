CREATE PROCEDURE [employer_account].[GetPayeSchemes_ByAccountId]
	@accountId BIGINT
AS
	SELECT 
		* 
	FROM 
		[employer_account].[Paye] p
	INNER JOIN 
		[employer_account].[AccountHistory] ah ON ah.PayeRef = p.Ref
	WHERE
		ah.AccountId = @accountId 
		AND ah.RemovedDate IS NULL
		AND p.Aorn IS NULL
