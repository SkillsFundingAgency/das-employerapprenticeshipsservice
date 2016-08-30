CREATE PROCEDURE [levy].[GetLevyDeclarations_ByAccountId]
	@accountId bigint = 0
AS
	select 
		x.* ,
		(y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * y.TopUpPercentage as TopUp
	FROM 
		[levy].[GetLevyDeclarations] x
	left join 
		[levy].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
	where
	x.AccountId = @accountId
	 order by SubmissionDate asc

