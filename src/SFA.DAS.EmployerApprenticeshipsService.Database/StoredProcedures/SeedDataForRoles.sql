Create Procedure [account].[SeedDataForRoles]
AS
BEGIN
	SET IDENTITY_INSERT  [account].[Role] ON 
	IF (NOT EXISTS(SELECT * FROM [account].[Role] WHERE Id = 1
		AND Name = 'Owner'))
	BEGIN 
		INSERT INTO [account].[Role](Id, Name) 
		VALUES(1, 'Owner') 
	END 
	ELSE 
	BEGIN 
		UPDATE [account].[Role] 
		SET Name = 'Owner'
		WHERE Id = 1
	END 
	IF (NOT EXISTS(SELECT * FROM [account].[Role] WHERE Id = 2
		AND Name = 'Transactor'))
	BEGIN 
		INSERT INTO [account].[Role](Id, Name) 
		VALUES(2, 'Transactor') 
	END 
	ELSE 
	BEGIN 
		UPDATE [account].[Role] 
		SET Name = 'Transactor'
		WHERE Id = 2
	END 
	IF (NOT EXISTS(SELECT * FROM [account].[Role] WHERE Id = 3
		AND Name = 'Viewer'))
	BEGIN 
		INSERT INTO [account].[Role](Id, Name) 
		VALUES(3, 'Viewer') 
	END 
	ELSE 
	BEGIN 
		UPDATE [account].[Role] 
		SET Name = 'Viewer'
		WHERE Id = 3
	END 
	SET IDENTITY_INSERT  [account].[Role] OFF

	SET IDENTITY_INSERT  [account].[EmployerAgreementStatus] ON 
	IF (NOT EXISTS(SELECT * FROM [account].[EmployerAgreementStatus] WHERE Id = 1
		AND Name = 'Pending'))
	BEGIN 
		INSERT INTO [account].[EmployerAgreementStatus](Id, Name) 
		VALUES(1, 'Pending') 
	END 
	ELSE 
	BEGIN 
		UPDATE [account].[EmployerAgreementStatus] 
		SET Name = 'Pending'
		WHERE Id = 1
	END 
	IF (NOT EXISTS(SELECT * FROM [account].[EmployerAgreementStatus] WHERE Id = 2
		AND Name = 'Signed'))
	BEGIN 
		INSERT INTO [account].[EmployerAgreementStatus](Id, Name) 
		VALUES(2, 'Signed') 
	END 
	ELSE 
	BEGIN 
		UPDATE [account].[EmployerAgreementStatus] 
		SET Name = 'Signed'
		WHERE Id = 2
	END 
	SET IDENTITY_INSERT  [account].[EmployerAgreementStatus] OFF

	SET IDENTITY_INSERT  [account].[EmployerAgreementTemplate] ON 
	IF (NOT EXISTS(SELECT * FROM [account].[EmployerAgreementTemplate] WHERE Id = 1
		AND [Text] = 'I am a template'))
	BEGIN 
		INSERT INTO [account].[EmployerAgreementTemplate](Id, [Text], CreatedDate, Ref) 
		VALUES(1, 'I am a template', GETDATE(), 'T/1') 
	END 
	ELSE 
	BEGIN 
		UPDATE [account].[EmployerAgreementTemplate] 
		SET [Text] = 'I am a template',
			Ref = 'T/1',
			CreatedDate = GETDATE()
		WHERE Id = 1
	END 

	SET IDENTITY_INSERT  [account].[EmployerAgreementTemplate] OFF
END
