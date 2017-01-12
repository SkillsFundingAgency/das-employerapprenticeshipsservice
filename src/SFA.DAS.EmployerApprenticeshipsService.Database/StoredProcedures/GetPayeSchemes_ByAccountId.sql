CREATE PROCEDURE [account].[GetPayeSchemes_ByAccountId]
	@accountId BIGINT
AS
	SELECT 
		* 
	FROM 
		[account].[Paye] p
	inner join 
		[account].[AccountHistory] ah on ah.PayeRef = p.Ref
	WHERE
		ah.AccountId = @accountId and ah.RemovedDate is null
