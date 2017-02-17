Create Procedure [employer_account].[SeedDataForRoles]
AS
BEGIN
	SET IDENTITY_INSERT  [employer_account].[Role] ON 
	IF (NOT EXISTS(SELECT * FROM [employer_account].[Role] WHERE Id = 1
		AND Name = 'Owner'))
	BEGIN 
		INSERT INTO [employer_account].[Role](Id, Name) 
		VALUES(1, 'Owner') 
	END 
	ELSE 
	BEGIN 
		UPDATE [employer_account].[Role] 
		SET Name = 'Owner'
		WHERE Id = 1
	END 
	IF (NOT EXISTS(SELECT * FROM [employer_account].[Role] WHERE Id = 2
		AND Name = 'Transactor'))
	BEGIN 
		INSERT INTO [employer_account].[Role](Id, Name) 
		VALUES(2, 'Transactor') 
	END 
	ELSE 
	BEGIN 
		UPDATE [employer_account].[Role] 
		SET Name = 'Transactor'
		WHERE Id = 2
	END 
	IF (NOT EXISTS(SELECT * FROM [employer_account].[Role] WHERE Id = 3
		AND Name = 'Viewer'))
	BEGIN 
		INSERT INTO [employer_account].[Role](Id, Name) 
		VALUES(3, 'Viewer') 
	END 
	ELSE 
	BEGIN 
		UPDATE [employer_account].[Role] 
		SET Name = 'Viewer'
		WHERE Id = 3
	END 
	SET IDENTITY_INSERT  [employer_account].[Role] OFF

	SET IDENTITY_INSERT  [employer_account].[EmployerAgreementStatus] ON 
	IF (NOT EXISTS(SELECT * FROM [employer_account].[EmployerAgreementStatus] WHERE Id = 1
		AND Name = 'Pending'))
	BEGIN 
		INSERT INTO [employer_account].[EmployerAgreementStatus](Id, Name) 
		VALUES(1, 'Pending') 
	END 
	ELSE 
	BEGIN 
		UPDATE [employer_account].[EmployerAgreementStatus] 
		SET Name = 'Pending'
		WHERE Id = 1
	END 
	IF (NOT EXISTS(SELECT * FROM [employer_account].[EmployerAgreementStatus] WHERE Id = 2
		AND Name = 'Signed'))
	BEGIN 
		INSERT INTO [employer_account].[EmployerAgreementStatus](Id, Name) 
		VALUES(2, 'Signed') 
	END 
	ELSE 
	BEGIN 
		UPDATE [employer_account].[EmployerAgreementStatus] 
		SET Name = 'Signed'
		WHERE Id = 2
	END 
	SET IDENTITY_INSERT  [employer_account].[EmployerAgreementStatus] OFF

	SET IDENTITY_INSERT  [employer_account].[EmployerAgreementTemplate] ON 
	IF (NOT EXISTS(SELECT * FROM [employer_account].[EmployerAgreementTemplate] WHERE Id = 1))
	BEGIN 
		INSERT INTO [employer_account].[EmployerAgreementTemplate](Id, PartialViewName, CreatedDate) 
		VALUES(1, '_Agreement_V1', GETDATE()) 
	END 
	ELSE 
	BEGIN 
		UPDATE [employer_account].[EmployerAgreementTemplate] 
		SET PartialViewName = '_Agreement_V1',
			CreatedDate = GETDATE()
		WHERE Id = 1
	END 

	SET IDENTITY_INSERT  [employer_account].[EmployerAgreementTemplate] OFF
END
