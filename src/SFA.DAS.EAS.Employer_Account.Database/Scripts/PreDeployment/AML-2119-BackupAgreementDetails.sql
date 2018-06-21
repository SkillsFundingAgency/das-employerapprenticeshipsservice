SET NOCOUNT ON;

IF EXISTS(SELECT	1 
			FROM	INFORMATION_SCHEMA.COLUMNS
		    WHERE	TABLE_SCHEMA = 'employer_account' AND
					TABLE_NAME = 'EmployerAgreement' AND
					COLUMN_NAME = 'AccountLegalEntityId')
BEGIN
	PRINT 'The EmployerAgreement table has already been updated - nothing to do';
END
ELSE 
BEGIN
	BEGIN TRY

		BEGIN TRANSACTION;
		PRINT 'The EmployerAgreement table has not been updated yet - creating agreement backup';

		IF(OBJECT_ID('employer_account.EmployerAgreement_Backup') IS NOT NULL)
		BEGIN
			DECLARE @backup_row_count as int,
					@master_row_count as int;

			SELECT	@backup_row_count = (SELECT COUNT(*) FROM employer_account.EmployerAgreement_Backup),
					@master_row_count = (SELECT COUNT(*) FROM employer_account.EmployerAgreement);

			IF @backup_row_count > 0 OR @master_row_count = 0
			BEGIN
				PRINT 'It looks like the employer agreement data has already been backed up - skipping this step';
				RETURN;
			END;

			DROP TABLE employer_account.EmployerAgreement_Backup;
		END
		
		SELECT *
		  INTO employer_account.EmployerAgreement_Backup
		  FROM employer_account.EmployerAgreement;

		PRINT 'Created backup of EmployerAgreements';

		DECLARE @fkeyExists as bit;
		SELECT @fkeyexists = CASE WHEN OBJECT_ID('[employer_account].[FK_AccountEmployerAgreement_EmployerAgreement]', 'F') IS NULL THEN 0 ELSE 1 END;

		IF @fkeyExists = 1
		BEGIN
			ALTER TABLE [employer_account].[AccountEmployerAgreement] 
				DROP CONSTRAINT [FK_AccountEmployerAgreement_EmployerAgreement];
		END;

		TRUNCATE TABLE employer_account.EmployerAgreement;

		PRINT 'Cleared down the EmployerAgreement table';

		IF @fkeyExists = 1
		BEGIN
			ALTER TABLE [employer_account].[AccountEmployerAgreement]  
				WITH CHECK ADD  CONSTRAINT [FK_AccountEmployerAgreement_EmployerAgreement] FOREIGN KEY([EmployerAgreementId])
				REFERENCES [employer_account].[EmployerAgreement] ([Id]);

			ALTER TABLE [employer_account].[AccountEmployerAgreement] 
				CHECK CONSTRAINT [FK_AccountEmployerAgreement_EmployerAgreement];
		END

		COMMIT TRANSACTION;

	END TRY
	BEGIN CATCH

		PRINT 'Error: could not backup employer agreement table';
		PRINT ERROR_MESSAGE();

		IF @@TRANCOUNT > 0
		BEGIN
			PRINT 'Rolling back transaction';
			ROLLBACK TRAN;
		END;

		THROW;

	END CATCH;
END;


