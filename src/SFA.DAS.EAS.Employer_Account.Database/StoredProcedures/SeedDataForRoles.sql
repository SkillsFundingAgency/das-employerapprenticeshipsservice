Create Procedure [employer_account].[SeedDataForRoles]
AS
BEGIN

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
