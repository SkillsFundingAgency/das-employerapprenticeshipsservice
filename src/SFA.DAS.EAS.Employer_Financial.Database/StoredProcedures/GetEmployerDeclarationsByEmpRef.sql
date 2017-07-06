CREATE PROCEDURE [employer_financial].[GetEmployerDeclarationsByEmpRef]
	@empRef NVARCHAR(50)
AS
	select 
		x.Id
	FROM 
		[employer_financial].[GetLevyDeclarationAndTopUp] x
	where
	x.EmpRef = @empRef
	order by SubmissionDate asc

