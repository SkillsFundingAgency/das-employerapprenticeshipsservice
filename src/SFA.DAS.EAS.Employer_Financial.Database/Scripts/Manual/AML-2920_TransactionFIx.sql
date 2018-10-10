
-- Replace values below with correct values and then run the script on a per tranaction basis
DECLARE @SubmissionId BIGINT = 1000
DECLARE @TransactionAmount DECIMAL = 1000
DECLARE @AdjustmentAmount DECIMAL = 1000


BEGIN TRY
BEGIN TRANSACTION

UPDATE employer_financial.TransactionLine
SET Amount = @TransactionAmount
WHERE SubmissionId = @SubmissionId

UPDATE employer_financial.LevyDeclaration
SET EndOfYearAdjustmentAmount = @AdjustmentAmount
WHERE SubmissionId = @SubmissionId

END TRY
BEGIN CATCH
	DECLARE @ErrorMsg nvarchar(max)
	DECLARE @ErrorSeverity INT;
	DECLARE @ErrorState INT;

	SET @ErrorMsg = ERROR_NUMBER() + ERROR_LINE() + ERROR_MESSAGE()
	SET @ErrorSeverity = ERROR_SEVERITY()
	SET @ErrorState = ERROR_STATE()

	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION

	RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState)
END CATCH

IF @@TRANCOUNT > 0
	COMMIT TRANSACTION
