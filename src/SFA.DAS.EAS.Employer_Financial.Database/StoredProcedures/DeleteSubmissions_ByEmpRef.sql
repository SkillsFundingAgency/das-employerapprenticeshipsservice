CREATE PROCEDURE [employer_financial].[DeleteSubmissions_ByEmpRef]
	 @empRef as nvarchar(50)
AS

	SET NOCOUNT ON

	DELETE	
	FROM	employer_financial.TransactionLine 
	WHERE	EmpRef = @empRef;

	DELETE ldt
	FROM	employer_financial.LevyDeclarationTopup ldt
	INNER JOIN	employer_financial.LevyDeclaration ld 
		On ld.SubmissionId= ldt.SubmissionId
	WHERE	ld.EmpRef = @empRef;

	DELETE 
	FROM	employer_financial.LevyDeclaration 
	WHERE	EmpRef = @empRef;

	
