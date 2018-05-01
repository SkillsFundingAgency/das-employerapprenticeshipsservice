CREATE TABLE [employer_account].[EmployerAgreement]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [LegalEntityId] BIGINT NOT NULL, 
	[AccountId] BIGINT NULL,
    [TemplateId] INT NOT NULL, 
    [StatusId] TINYINT NOT NULL DEFAULT 1, 
    [SignedByName] NVARCHAR(100) NULL, 
    [SignedDate] DATETIME NULL, 
    [ExpiredDate] DATETIME NULL, 
    [SignedById] BIGINT NULL, 
    CONSTRAINT [FK_EmployerAgreement_LegalEntity] FOREIGN KEY ([LegalEntityId]) REFERENCES [employer_account].[LegalEntity]([Id]), 
    CONSTRAINT [FK_EmployerAgreement_Account] FOREIGN KEY ([AccountId]) REFERENCES [employer_account].[Account]([Id]), 
    CONSTRAINT [FK_EmployerAgreement_SignedBy] FOREIGN KEY ([SignedById]) REFERENCES [employer_account].[User]([Id]),
)
GO

CREATE INDEX [IX_EmployerAgreement_AccountId_StatusId]
ON [employer_account].[EmployerAgreement] ([AccountId], [StatusId])
GO

CREATE UNIQUE INDEX [IX_EmployerAgreement_LegalEntityId_AccountId_TemplateId]
ON [employer_account].[EmployerAgreement] ([LegalEntityId], [AccountId], [TemplateId])
WHERE [StatusId] <> 5
GO