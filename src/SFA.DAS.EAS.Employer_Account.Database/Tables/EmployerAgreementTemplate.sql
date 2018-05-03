CREATE TABLE [employer_account].[EmployerAgreementTemplate]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PartialViewName] NVARCHAR(50) NOT NULL,
    [CreatedDate] DATETIME NOT NULL, 
    [VersionNumber] INT NOT NULL
)
GO

CREATE UNIQUE INDEX [IX_VersionNumber]
ON [employer_account].[EmployerAgreementTemplate] ([VersionNumber] DESC)
GO
