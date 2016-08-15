CREATE TABLE [dbo].[EmployerAgreementTemplate]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[Ref] NVARCHAR(50) NOT NULL,
    [Text] NVARCHAR(MAX) NOT NULL, 
    [CreatedDate] DATETIME NOT NULL,
	[ReleasedDate] DATETIME NULL
)
