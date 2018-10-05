CREATE PROCEDURE [employer_financial].[DeleteSubmissions_ByEmpRef]
	 @empRef as nvarchar(50)
AS

	SET NOCOUNT ON

	DELETE	
	FROM	employer_financial.TransactionLine 
	WHERE	EmpRef = @empRef;

	DELETE 
	FROM	employer_financial.LevyDeclaration 
	WHERE	EmpRef = empRef;

