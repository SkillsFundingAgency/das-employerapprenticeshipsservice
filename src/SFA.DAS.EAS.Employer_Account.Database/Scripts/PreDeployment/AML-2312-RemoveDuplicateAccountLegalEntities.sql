
IF (EXISTS (SELECT *  
					FROM INFORMATION_SCHEMA.TABLES  
					WHERE TABLE_SCHEMA = 'employer_account'  
					AND  TABLE_NAME = 'AccountEmployerAgreement')) 
BEGIN 
	DELETE FROM  [employer_account].[AccountEmployerAgreement]
END


-- Store the original data 
IF (EXISTS (SELECT *  
				FROM INFORMATION_SCHEMA.TABLES  
				WHERE TABLE_SCHEMA = 'employer_account')
	AND
	
	NOT EXISTS (SELECT *  
					FROM INFORMATION_SCHEMA.TABLES  
					WHERE TABLE_SCHEMA = 'employer_account'  
					AND  TABLE_NAME = 'LegalEntityNonUnique')) 
BEGIN 

	SELECT le.Name, le.Code, le.RegisteredAddress, le.DateOfIncorporation, le.Status, le.Source, le.PublicSectorDataSource, le.Sector
		INTO [employer_account].[LegalEntityNonUnique]
	FROM employer_account.LegalEntity le
	WHERE Code IN (
		SELECT Code
		FROM employer_account.LegalEntity
		GROUP BY Code
		HAVING COUNT(1) > 1)
	ORDER BY le.Name, Code, le.RegisteredAddress

	-- Update duplicate LegalEntityId's in EmployerAgreement table

	-- To do this we need to Group The Legal Entities together and update the table to 
	-- use the first Id with this code
	Update ea
	SET ea.LegalEntityId = dupes.Id
	--Select *
	FROM employer_account.EmployerAgreement as ea
	INNER JOIN employer_account.LegalEntity le
		ON ea.LegalEntityId = le.Id
	INNER JOIN (SELECT Min(Id) as Id, Code
					FROM employer_account.LegalEntity
				WHERE Code IN (
					SELECT Code
					FROM employer_account.LegalEntity
					GROUP BY Code
					HAVING COUNT(1) > 1
				)
				GROUP By Code) dupes
		ON dupes.Code = le.Code AND dupes.Id <> ea.LegalEntityId


	-- Delete the duplicate Legal Entities
	DELETE le
	FROM employer_account.LegalEntity le
	INNER JOIN (SELECT Min(Id) as Id, Code
					FROM employer_account.LegalEntity
				WHERE Code IN (
					SELECT Code
					FROM employer_account.LegalEntity
					GROUP BY Code
					HAVING COUNT(1) > 1
				)
				GROUP By Code) dupes
		ON dupes.Code = le.Code AND dupes.Id <> le.Id


	-- Now we could have ended up with duplicates in the EmployerAgreement table after the update 

	-- Clean any dupliacates
		;WITH duplicateRow as (SELECT Id, LegalEntityId, AccountId,  TemplateId, StatusId, row_number() 
		OVER(PARTITION BY LegalEntityId, AccountId,TemplateId ORDER BY SignedDate DESC) AS [rn]
		FROM [employer_account].[EmployerAgreement])

		DELETE FROM [employer_account].[EmployerAgreement]
		WHERE Id IN 
		( 
			SELECT Id FROM duplicateRow 
			WHERE rn > 1
			AND StatusId <> 5
		)
END
GO

