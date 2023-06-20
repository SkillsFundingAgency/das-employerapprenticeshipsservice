CREATE PROCEDURE [employer_account].[GetPayeSchemes_ByAccountId]
	@accountId BIGINT
AS
	SELECT 
		* 
	FROM 
		[employer_account].[Paye] p
	inner join 
		[employer_account].[AccountHistory] ah on ah.PayeRef = p.Ref
	WHERE
		ah.AccountId = @accountId and ah.RemovedDate is null
