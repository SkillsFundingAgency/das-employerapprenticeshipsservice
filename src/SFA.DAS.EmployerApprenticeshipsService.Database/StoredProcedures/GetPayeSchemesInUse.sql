CREATE PROCEDURE [account].[GetPayeSchemesInUse]
	@payeRef varchar(20)
AS
	SELECT 
		* 
	FROM 
		[account].[Paye] p
	inner join 
		[account].[AccountHistory] ah on ah.PayeRef = p.Ref
	WHERE
		ah.RemovedDate is null and p.Ref = @payeRef
