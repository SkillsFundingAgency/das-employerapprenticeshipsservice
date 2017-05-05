CREATE PROCEDURE [employer_financial].[UpdateTransactionLineDate_BySubmissionId]
	@submissionId bigint,
	@createdDate datetime
AS
	UPDATE employer_financial.TransactionLine 
	set DateCreated = @createdDate 
	where SubmissionId = @submissionId
