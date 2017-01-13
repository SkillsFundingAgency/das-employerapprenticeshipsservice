CREATE PROCEDURE [employer_transactions].[GetLastLevyDeclarations_ByEmpRef]
	@empRef varchar(20)
AS
	select top 1
		*
	FROM 
		[employer_transactions].[LevyDeclaration]
	where
		EmpRef = @empRef
	order by SubmissionDate desc
