IF (NOT EXISTS (SELECT 1 FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V1'))
BEGIN 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] ON
	INSERT INTO [employer_account].[EmployerAgreementTemplate] (Id, PartialViewName, VersionNumber, CreatedDate)
	VALUES (1, '_Agreement_V1', 1, GETDATE()) 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] OFF
END
