SET NOCOUNT ON;

PRINT 'Add new NonLevy.EOI EmployerAgreementTemplate entry';

BEGIN TRAN;
BEGIN TRY

	IF OBJECT_ID('employer_account.EmployerAgreementTemplate') IS NULL
	BEGIN;
		THROW 50000, 'The employer agreement template could not be added as the employer_account.AccountLegalEntity table does not exist', 1;
	END;

	IF NOT EXISTS(SELECT ID FROM employer_account.EmployerAgreementTemplate WHERE [AgreementType] = 'NonLevy.EOI')
	BEGIN
		INSERT INTO employer_account.EmployerAgreementTemplate 
				(	[PartialViewName],
					[CreatedDate], 
					[VersionNumber],
					[AgreementType]
				)

			SELECT	'_NonLevy_Agreement_V1',
					GETDATE(),
					-- Although not ideal, making the version number unique across agreement types will mean any orderby logic in the codebase can remain unchanged and still get the
					-- correct data
					(SELECT MAX(VersionNumber) FROM employer_account.EmployerAgreementTemplate) + 1,
					'NonLevy.EOI'

		COMMIT TRAN;
		PRINT 'NonLevy.EOI EmployerAgreementTemplate entry added';
	END
	ELSE
	BEGIN
		ROLLBACK TRAN;
		PRINT 'NonLevy.EOI EmployerAgreementTemplate entry already added';
	END

END TRY
BEGIN CATCH
	
	PRINT Error_message();
	PRINT 'NonLevy.EOI EmployerAgreementTemplate entry NOT added see above error';
	PRINT 'Rolling back transaction';
	
	ROLLBACK TRAN;

	THROW;
END CATCH;
