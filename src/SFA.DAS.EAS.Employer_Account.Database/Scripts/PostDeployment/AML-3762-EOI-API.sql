SET NOCOUNT ON;

PRINT 'Add new EOI EmployerAgreementTemplate entry';

BEGIN TRAN;
BEGIN TRY

	IF OBJECT_ID('employer_account.EmployerAgreementTemplate') IS NULL
	BEGIN;
		THROW 50000, 'The employer agreement template could not be added as the employer_account.EmployerAgreementTemplate table does not exist', 1;
	END;

	IF NOT EXISTS(SELECT ID FROM employer_account.EmployerAgreementTemplate WHERE [AgreementType] = 1)
	BEGIN
		SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] ON

		INSERT INTO employer_account.EmployerAgreementTemplate 
				(	
					Id,
					[PartialViewName],
					[CreatedDate], 
					[VersionNumber],
					[AgreementType]
				)

			SELECT	3,
					'_NonLevy_Agreement_V1',
					GETDATE(),
					1,
					1

		SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] OFF

		COMMIT TRAN;
		PRINT 'EOI EmployerAgreementTemplate entry added';
	END
	ELSE
	BEGIN
		ROLLBACK TRAN;
		PRINT 'EOI EmployerAgreementTemplate entry already added';
	END

END TRY
BEGIN CATCH
	
	PRINT Error_message();
	PRINT 'EOI EmployerAgreementTemplate entry NOT added see above error';
	PRINT 'Rolling back transaction';
	
	ROLLBACK TRAN;

	THROW;
END CATCH;
