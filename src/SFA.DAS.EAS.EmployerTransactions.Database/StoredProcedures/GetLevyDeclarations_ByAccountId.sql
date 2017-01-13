CREATE PROCEDURE [employer_transactions].[GetLevyDeclarations_ByAccountId]
	@accountId bigint = 0
AS
	select 
		x.* ,
		(y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * y.TopUpPercentage as TopUp
	FROM 
		[employer_transactions].[GetLevyDeclarations] x
	left join 
		[employer_transactions].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
	where
	x.EmpRef in (Select Empref from [employer_transactions].LevyDeclaration where AccountId = @accountId)
	 order by SubmissionDate asc

