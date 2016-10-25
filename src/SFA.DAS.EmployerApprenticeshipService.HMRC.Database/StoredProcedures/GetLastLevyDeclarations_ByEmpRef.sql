CREATE PROCEDURE [levy].[GetLastLevyDeclarations_ByEmpRef]
	@empRef varchar(20)
AS
	select top 1
		*
	FROM 
		[levy].[LevyDeclaration]
	where
		EmpRef = @empRef
	order by SubmissionDate desc
