CREATE PROCEDURE [employer_financial].[UpdateTransactionLineDateCreatedToTransactionDate_BySubmissionId]
	 @SubmissionIds [employer_financial].[SubmissionIds] READONLY
AS
	UPDATE employer_financial.TransactionLine 
	SET DateCreated = TransactionDate 
	WHERE SubmissionId IN (
		SELECT SubmissionId FROM @SubmissionIds t
	)
