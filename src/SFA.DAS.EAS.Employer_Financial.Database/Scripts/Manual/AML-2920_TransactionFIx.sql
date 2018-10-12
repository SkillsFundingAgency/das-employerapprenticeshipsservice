
-- Replace values below with correct values and then run the script on a per transaction basis
DECLARE @SubmissionId BIGINT = NULL
DECLARE @TransactionLevyAmount DECIMAL = NULL
DECLARE @TransactionAmount DECIMAL = NULL
DECLARE @AdjustmentAmount DECIMAL = NULL
DECLARE @TopupAmount DECIMAL = NULL


BEGIN TRY
BEGIN TRANSACTION

UPDATE employer_financial.TransactionLine
SET Amount = @TransactionAmount,
LevyDeclared = @TransactionLevyAmount
WHERE SubmissionId = @SubmissionId

UPDATE employer_financial.LevyDeclaration
SET EndOfYearAdjustmentAmount = @AdjustmentAmount
WHERE SubmissionId = @SubmissionId

UPDATE [employer_financial].LevyDeclarationTopup
SET [Amount] = @TopupAmount 
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
