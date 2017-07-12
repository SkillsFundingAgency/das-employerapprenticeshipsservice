CREATE PROCEDURE [employer_financial].[GetLevyDeclarationSubmissionIdsByEmpRef]
	@empRef NVARCHAR(50)
AS
	select 
		x.SubmissionId
	FROM 
		[employer_financial].[GetLevyDeclarationAndTopUp] x
	where
	x.EmpRef = @empRef
	order by SubmissionDate asc

