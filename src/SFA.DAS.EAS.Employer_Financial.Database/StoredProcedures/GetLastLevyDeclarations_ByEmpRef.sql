CREATE PROCEDURE [employer_financial].[GetLastLevyDeclarations_ByEmpRef]
	@empRef varchar(20)
AS
	select top 1
		*
	FROM 
		[employer_financial].[LevyDeclaration]
	where
		EmpRef = @empRef
	order by SubmissionDate desc
