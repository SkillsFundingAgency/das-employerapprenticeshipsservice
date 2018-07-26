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

		IF(EXISTS(SELECT TOP 1 * FROM employer_account.AccountLegalEntity))
		BEGIN
			PRINT 'It looks like the account-legal entity has already been restored - skipping this step';
			RETURN;
		END;

		PRINT '>>TranCount:' + convert(varchar(10), @@TranCount);

		BEGIN TRAN;

			INSERT 
				INTO employer_account.AccountLegalEntity (Name, Address, SignedAgreementVersion, SignedAgreementId, PendingAgreementVersion, PendingAgreementId, AccountId, LegalEntityId, Created)

				SELECT	bak.Name, 
						bak.RegisteredAddress, 
						Signed.VersionNumber,
						Signed.AgreementId, 
						Pending.VersionNumber,
						Pending.AgreementId, 
						bak.AccountId, 
						bak.LegalEntityId, 
						GetUtcDate()
				FROM	(SELECT Distinct AccountId, LegalEntityId, Name, RegisteredAddress FROM employer_account.EmployerAgreement_Backup) AS bak
						JOIN employer_account.LegalEntity AS LE
							ON LE.Id = bak.LegalEntityId
						JOIN employer_account.Account AS AC
							ON AC.Id = bak.AccountId

						OUTER APPLY (SELECT	TOP 1 sa.Id AS AgreementId, sa.AccountId, sa.LegalEntityId, et.VersionNumber
									FROM	employer_account.EmployerAgreement_Backup AS sa 
											JOIN employer_account.EmployerAgreementTemplate as ET
												ON ET.Id = sa.TemplateId
									WHERE	sa.AccountId = bak.AccountId
											AND sa.LegalEntityId = bak.LegalEntityId
											AND sa.StatusId = 2
									ORDER BY ET.VersionNumber DESC) AS Signed

						OUTER APPLY (SELECT	TOP 1 pa.Id AS AgreementId, pa.AccountId, pa.LegalEntityId, et.VersionNumber
									FROM	employer_account.EmployerAgreement_Backup AS pa 
											JOIN employer_account.EmployerAgreementTemplate as ET
												ON ET.Id = pa.TemplateId
									WHERE	pa.AccountId = bak.AccountId
											AND pa.LegalEntityId = bak.LegalEntityId
											AND pa.StatusId = 1 
									ORDER BY ET.VersionNumber DESC) AS Pending
				
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

		PRINT '<<OK: TranCount:' + convert(varchar(10), @@TranCount);

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

		PRINT '<<Error: TranCount:' + convert(varchar(10), @@TranCount);

		THROW;
	END CATCH;
END;