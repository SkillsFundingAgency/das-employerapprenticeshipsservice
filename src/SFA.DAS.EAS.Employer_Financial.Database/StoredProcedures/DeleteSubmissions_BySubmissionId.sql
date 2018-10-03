CREATE PROCEDURE [employer_financial].[DeleteSubmissions_BySubmissionId]
	 @SubmissionIds [employer_financial].[SubmissionIds] READONLY
AS
SET NOCOUNT ON

	DELETE tl
	FROM employer_financial.TransactionLine tl
	INNER JOIN @SubmissionIds dts
		ON tl.SubmissionId = dts.SubmissionId

	DELETE ld
	FROM employer_financial.LevyDeclaration ld
	INNER JOIN @SubmissionIds dts
		ON ld.SubmissionId = dts.SubmissionId