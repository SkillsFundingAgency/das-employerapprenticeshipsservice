IF (NOT EXISTS (SELECT 1 FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V1'))
BEGIN 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] ON
	INSERT INTO [employer_account].[EmployerAgreementTemplate] (Id, PartialViewName, VersionNumber, CreatedDate, PublishedDate)
	VALUES (1, '_Agreement_V1', 1, GETDATE(), Convert(DateTime,'2017, 5, 1')) 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] OFF
END

IF (NOT EXISTS (SELECT 1 FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V2'))
BEGIN 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] ON
	INSERT INTO [employer_account].[EmployerAgreementTemplate] (Id, PartialViewName, VersionNumber, CreatedDate, PublishedDate)
	VALUES (2, '_Agreement_V2', 2, GETDATE(), Convert(DateTime,'2018, 5, 1')) 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] OFF
END

IF (NOT EXISTS (SELECT 1 FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V3'))
BEGIN 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] ON
	INSERT INTO [employer_account].[EmployerAgreementTemplate] (Id, PartialViewName, VersionNumber, CreatedDate, PublishedDate)
	VALUES (4, '_Agreement_V3', 3, GETDATE(), Convert(DateTime,'2020, 1, 9')) 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] OFF
END