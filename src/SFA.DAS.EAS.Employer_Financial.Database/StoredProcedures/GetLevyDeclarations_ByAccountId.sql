CREATE PROCEDURE [employer_financial].[GetLevyDeclarations_ByAccountId]
	@accountId bigint = 0
AS
	select 
		x.* ,
		(y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * y.TopUpPercentage as TopUp
	FROM 
		[employer_financial].[GetLevyDeclarations] x
	left join 
		[employer_financial].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
	where
	x.EmpRef in (Select Empref from [employer_financial].LevyDeclaration where AccountId = @accountId)
	 order by SubmissionDate asc

