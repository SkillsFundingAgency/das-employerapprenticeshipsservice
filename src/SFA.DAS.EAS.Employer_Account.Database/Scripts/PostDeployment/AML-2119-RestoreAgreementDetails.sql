SET NOCOUNT ON;

IF OBJECT_ID('employer_account.EmployerAgreement_Backup') IS NULL
BEGIN
	PRINT 'No employer agreement data backup exists - nothing to do';
END;
ELSE
BEGIN

	PRINT 'Restoring agreement links';

	BEGIN TRY

		IF OBJECT_ID('employer_account.AccountLegalEntity') IS NULL
		BEGIN;
			THROW 50000, 'The employer agreement backup cannot be restored because the employer_account.AccountLegalEntity table does not exist - has the database been upgraded?', 1;
		END;

		BEGIN TRAN;

			INSERT 
				INTO employer_account.AccountLegalEntity (Name, Address, CurrentSignedAgreement, CurrentPendingAgreement, AccountId, LegalEntityId, Created)

				SELECT	LE.Name, 
						LE.RegisteredAddress, 
						(	SELECT	MAX(Id) AS AgreementId
									FROM	employer_account.EmployerAgreement_Backup AS sa 
									WHERE	sa.AccountId = bak.AccountId
											AND sa.LegalEntityId = bak.AccountId
											AND sa.StatusId = 2 ), 
						(	SELECT	MAX(Id) AS AgreementId
									FROM	employer_account.EmployerAgreement_Backup AS pa 
									WHERE	pa.AccountId = bak.AccountId
											AND pa.LegalEntityId = bak.AccountId
											AND pa.StatusId = 1 ),
						bak.AccountId, 
						bak.LegalEntityId, 
						GetDate()
				FROM	(SELECT Distinct AccountId, LegalEntityId FROM employer_account.EmployerAgreement_Backup) AS bak
						JOIN employer_account.LegalEntity AS LE
							ON LE.Id = bak.LegalEntityId
						JOIN employer_account.Account AS AC
							ON AC.Id = bak.AccountId;

				
				SET IDENTITY_INSERT employer_account.EmployerAgreement ON;

				INSERT 
					INTO	employer_account.EmployerAgreement (Id, AccountLegalEntityId, TemplateId, StatusId, SignedByName, SignedDate, ExpiredDate, SignedById)
					
					SELECT	BAK.Id, ALE.Id, BAK.TemplateId, BAK.StatusId, BAK.SignedByName, BAK.SignedDate, BAK.ExpiredDate, BAK.SignedById
					FROM	employer_account.EmployerAgreement_Backup AS BAK
							JOIN employer_account.AccountLegalEntity AS ALE
								ON ALE.AccountId = BAK.AccountId
									AND ALE.LegalEntityId = BAK.LegalEntityId;

				SET IDENTITY_INSERT employer_account.EmployerAgreement OFF;

		COMMIT TRAN;

		PRINT 'Restore agreement links successful';
	
	END TRY
	BEGIN CATCH
		PRINT 'Restore of employer agreement backup failed';
		PRINT Error_message();

		IF @@TRANCOUNT > 0
		BEGIN
			PRINT 'Rolling back transaction';
			ROLLBACK TRAN;
		END;

		THROW;
	END CATCH;
END;