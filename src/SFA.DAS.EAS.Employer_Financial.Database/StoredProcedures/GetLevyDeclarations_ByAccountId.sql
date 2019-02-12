CREATE PROCEDURE [employer_financial].[GetLevyDeclarations_ByAccountId]
	@AccountId bigint = 0
AS
	select 
		x.*
	FROM 
		[employer_financial].[GetLevyDeclarationAndTopUp] x
	where
	x.EmpRef in (Select EmpRef from [employer_financial].LevyDeclaration where AccountId = @AccountId)
	AND (x.LastSubmission = 1 OR x.EndOfYearAdjustment = 1)
	order by SubmissionDate asc

