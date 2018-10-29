BEGIN TRY
BEGIN TRANSACTION

DELETE FROM [employer_financial].[Payment] WHERE PeriodEnd = '1718-R14'
DELETE FROM [employer_financial].[PaymentMetaData] WHERE Id NOT IN (SELECT PaymentMetaDataId FROM [employer_financial].[Payment])
DELETE FROM [employer_financial].[TransactionLine] WHERE PeriodEnd = '1718-R14'
DELETE FROM [employer_financial].[PeriodEnd] WHERE PeriodEndId = '1718-R14'

IF @@TRANCOUNT > 0
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
DECLARE @ErrorMsg nvarchar(max);
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  

	SET @ErrorMsg = '[Code: ' + CAST(ERROR_NUMBER() AS VARCHAR) + ', Line: ' + CAST(ERROR_LINE() AS VARCHAR) + ' ] ' + ERROR_MESSAGE()
	SET @ErrorSeverity = ERROR_SEVERITY()
	SET @ErrorState = ERROR_STATE()

	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION 
	
	RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState)
END CATCH