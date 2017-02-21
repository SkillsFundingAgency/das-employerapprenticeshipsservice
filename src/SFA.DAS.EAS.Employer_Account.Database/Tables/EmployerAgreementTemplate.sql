CREATE TABLE [employer_account].[EmployerAgreementTemplate]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[PartialViewName] NVARCHAR(50) NOT NULL DEFAULT('_Agreement_v1'),
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE()
)
