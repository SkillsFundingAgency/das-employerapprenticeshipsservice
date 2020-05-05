CREATE TABLE [employer_account].[EmployerAgreementTemplate]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PartialViewName] NVARCHAR(50) NOT NULL,
    [CreatedDate] DATETIME NOT NULL, 
    [VersionNumber] INT NOT NULL,
	[AgreementType] TINYINT NOT NULL DEFAULT 0,
	[PublishedDate]  DATETIME NULL
)
GO

CREATE UNIQUE INDEX [IX_AgreementTypeVersionNumber]
ON [employer_account].[EmployerAgreementTemplate] (AgreementType, [VersionNumber] DESC)
GO
