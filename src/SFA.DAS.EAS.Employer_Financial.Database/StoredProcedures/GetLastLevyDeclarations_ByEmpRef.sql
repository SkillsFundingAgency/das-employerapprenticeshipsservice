CREATE PROCEDURE [employer_financial].[GetLastLevyDeclarations_ByEmpRef]
	@EmpRef varchar(20)
AS
	select top 1
		*
	FROM 
		[employer_financial].[LevyDeclaration]
	where
		EmpRef = @EmpRef
	order by SubmissionDate desc
