CREATE TABLE [employer_account].[EmployerAgreement]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [LegalEntityId] BIGINT NOT NULL, 
    [TemplateId] INT NOT NULL, 
    [StatusId] TINYINT NOT NULL DEFAULT 1, 
    [SignedByName] NVARCHAR(100) NULL, 
    [SignedDate] DATETIME NULL, 
    [ExpiredDate] DATETIME NULL, 
    [SignedById] BIGINT NULL, 
    CONSTRAINT [FK_EmployerAgreement_LegalEntity] FOREIGN KEY ([LegalEntityId]) REFERENCES [employer_account].[LegalEntity]([Id]), 
    CONSTRAINT [FK_EmployerAgreement_Template] FOREIGN KEY ([TemplateId]) REFERENCES [employer_account].[EmployerAgreementTemplate]([Id]), 
    CONSTRAINT [FK_EmployerAgreement_SignedBy] FOREIGN KEY ([SignedById]) REFERENCES [employer_account].[User]([Id]),
    CONSTRAINT [FK_EmployerAgreement_Status] FOREIGN KEY ([StatusId]) REFERENCES [employer_account].[EmployerAgreementStatus]([Id])
)
