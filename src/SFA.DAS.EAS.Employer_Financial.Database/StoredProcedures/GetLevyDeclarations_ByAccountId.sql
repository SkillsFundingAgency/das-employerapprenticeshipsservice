CREATE PROCEDURE [employer_financial].[GetLevyDeclarations_ByAccountId]
	@accountId bigint = 0
AS
	select 
		x.*
	FROM 
		[employer_financial].[GetLevyDeclarationAndTopUp] x
	where
	x.EmpRef in (Select Empref from [employer_financial].LevyDeclaration where AccountId = @accountId)
	AND x.LastSubmission = 1
	order by SubmissionDate asc

