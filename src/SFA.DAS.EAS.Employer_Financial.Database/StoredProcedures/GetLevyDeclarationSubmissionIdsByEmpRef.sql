CREATE PROCEDURE [employer_financial].[GetLevyDeclarationSubmissionIdsByEmpRef]
	@EmpRef NVARCHAR(50)
AS
	select 
		x.SubmissionId
	FROM 
		[employer_financial].[GetLevyDeclarationAndTopUp] x
	where
	x.EmpRef = @EmpRef
	order by SubmissionDate asc

