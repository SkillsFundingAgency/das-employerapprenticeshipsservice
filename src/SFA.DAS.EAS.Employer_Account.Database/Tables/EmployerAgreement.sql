CREATE TABLE [employer_account].[EmployerAgreement]
(
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [TemplateId] INT NOT NULL, 
    [StatusId] TINYINT NOT NULL DEFAULT 1, 
    [SignedByName] NVARCHAR(100) NULL, 
    [SignedDate] DATETIME NULL, 
	[AccountLegalEntityId] BIGINT NOT NULL,
    [ExpiredDate] DATETIME NULL, 
    [SignedById] BIGINT NULL, 
    CONSTRAINT [FK_EmployerAgreement_AccountLegalEntity] FOREIGN KEY ([AccountLegalEntityId]) REFERENCES [employer_account].[AccountLegalEntity]([Id]), 
    CONSTRAINT [FK_EmployerAgreement_SignedBy] FOREIGN KEY ([SignedById]) REFERENCES [employer_account].[User]([Id])
)
GO

CREATE UNIQUE INDEX [IX_EmployerAgreement_LegalEntity]
ON [employer_account].[EmployerAgreement] (AccountLegalEntityId, TemplateId)
WHERE StatusId <> 5
GO
