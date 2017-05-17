CREATE PROCEDURE [employer_financial].[GetLevyDeclarations_ByAccountId]
	@accountId bigint = 0
AS
	select 
		x.*
	FROM 
		[employer_financial].[GetLevyDeclarationAndTopUp] x
	where
	x.EmpRef in (Select Empref from [employer_financial].LevyDeclaration where AccountId = @accountId)
	order by SubmissionDate asc

