CREATE PROCEDURE [employer_financial].[UpdateTransactionLinesDateCreated_BySubmissionId]
	 @SubmissionIdsDates [employer_financial].[SubmissionIdsDate] READONLY
AS
	UPDATE tl
	SET tl.DateCreated = dts.CreatedDate
	FROM employer_financial.TransactionLine tl
	INNER JOIN @SubmissionIdsDates dts
		ON tl.SubmissionId = dts.SubmissionId
