CREATE TABLE [dbo].[EmployerAgreementTemplate]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Text] NVARCHAR(MAX) NOT NULL, 
    [CreatedDate] DATETIME NOT NULL
)
