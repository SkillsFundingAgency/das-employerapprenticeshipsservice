IF (NOT EXISTS (SELECT 1 FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V1'))
BEGIN 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] ON
	INSERT INTO [employer_account].[EmployerAgreementTemplate] (Id, PartialViewName, VersionNumber, CreatedDate, PublishedDate)
	VALUES (1, '_Agreement_V1', 1, GETDATE(), Convert(DateTime,'2017-5-1')) 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] OFF
END

IF (NOT EXISTS (SELECT 1 FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V2'))
BEGIN 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] ON
	INSERT INTO [employer_account].[EmployerAgreementTemplate] (Id, PartialViewName, VersionNumber, CreatedDate, PublishedDate)
	VALUES (2, '_Agreement_V2', 2, GETDATE(), Convert(DateTime,'2018-5-1')) 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] OFF
END

IF (NOT EXISTS (SELECT 1 FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V3'))
BEGIN 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] ON
	INSERT INTO [employer_account].[EmployerAgreementTemplate] (Id, PartialViewName, VersionNumber, CreatedDate, PublishedDate)
	VALUES (4, '_Agreement_V3', 3, GETDATE(), Convert(DateTime,'2020-1-9')) 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] OFF
END

IF (NOT EXISTS (SELECT 1 FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V4'))
BEGIN 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] ON
	INSERT INTO [employer_account].[EmployerAgreementTemplate] (Id, PartialViewName, VersionNumber, AgreementType, CreatedDate, PublishedDate)
	VALUES (5, '_Agreement_V4', 4, 2, GETDATE(), Convert(DateTime,'2020-8-20')) 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] OFF
END

IF (NOT EXISTS (SELECT 1 FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V5'))
BEGIN 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] ON
	INSERT INTO [employer_account].[EmployerAgreementTemplate] (Id, PartialViewName, VersionNumber, AgreementType, CreatedDate, PublishedDate)
	VALUES (6, '_Agreement_V5', 5, 2, GETDATE(), Convert(DateTime,'2021-1-20')) 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] OFF
END

IF (NOT EXISTS (SELECT 1 FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V6'))
BEGIN 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] ON
	INSERT INTO [employer_account].[EmployerAgreementTemplate] (Id, PartialViewName, VersionNumber, AgreementType, CreatedDate, PublishedDate)
	VALUES (7, '_Agreement_V6', 6, 2, GETDATE(), Convert(DateTime,'2021-5-19')) 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] OFF
END

IF (NOT EXISTS (SELECT 1 FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V7'))
BEGIN 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] ON
	INSERT INTO [employer_account].[EmployerAgreementTemplate] (Id, PartialViewName, VersionNumber, AgreementType, CreatedDate, PublishedDate)
	VALUES (8, '_Agreement_V7', 7, 2, GETDATE(), Convert(DateTime,'2022-01-11')) 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] OFF
END

IF (NOT EXISTS (SELECT 1 FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V8'))
BEGIN 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] ON
	INSERT INTO [employer_account].[EmployerAgreementTemplate] (Id, PartialViewName, VersionNumber, AgreementType, CreatedDate, PublishedDate)
	VALUES (9, '_Agreement_V8', 8, 2, GETDATE(), Convert(DateTime,'2022-10-26')) 
	SET IDENTITY_INSERT [employer_account].[EmployerAgreementTemplate] OFF
END