CREATE PROCEDURE [employer_financial].[DeleteSubmissions_ByEmpRef]
	 @empRef as nvarchar(50)
AS

	SET NOCOUNT ON

	DECLARE @SubmissionIdsInUseByEmpRef AS table(submissionId bigint);

	INSERT INTO @SubmissionIdsInUseByEmpRef
	SELECT	SubmissionId
	FROM	employer_financial.LevyDeclaration
	WHERE	EmpRef = @empRef;

	DELETE	
	FROM	employer_financial.TransactionLine 
	WHERE	SubmissionId IN (SELECT SubmissionId FROM @SubmissionIdsInUseByEmpRef);

	DELETE 
	FROM	employer_financial.LevyDeclarationTopup
	WHERE	SubmissionId IN (SELECT SubmissionId FROM @SubmissionIdsInUseByEmpRef);

	DELETE 
	FROM	employer_financial.LevyDeclaration 
	WHERE	SubmissionId IN (SELECT SubmissionId FROM @SubmissionIdsInUseByEmpRef);

	
