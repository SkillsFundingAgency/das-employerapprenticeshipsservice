CREATE TABLE [employer_account].[EmployerAgreementTemplate]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[Ref] NVARCHAR(50) NOT NULL,
    [Text] NVARCHAR(MAX) NOT NULL, 
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
	[ReleasedDate] DATETIME NULL, 
    [ExpiryDays] INT NULL DEFAULT 30
)
