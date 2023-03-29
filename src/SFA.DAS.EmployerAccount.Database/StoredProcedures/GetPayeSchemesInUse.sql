CREATE PROCEDURE [employer_account].[GetPayeSchemesInUse]
	@payeRef varchar(20)
AS
	SELECT 
		* 
	FROM 
		[employer_account].[Paye] p
	inner join 
		[employer_account].[AccountHistory] ah on ah.PayeRef = p.Ref
	WHERE
		ah.RemovedDate is null and p.Ref = @payeRef
