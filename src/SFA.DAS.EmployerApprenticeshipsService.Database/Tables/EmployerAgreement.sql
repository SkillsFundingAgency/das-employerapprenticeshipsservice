CREATE TABLE [dbo].[EmployerAgreement]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [LegalEntityId] BIGINT NOT NULL, 
    [TemplateId] INT NOT NULL, 
    [StatusId] TINYINT NOT NULL DEFAULT 1, 
    [SignedByName] NVARCHAR(100) NULL, 
    [SignedDate] DATETIME NULL, 
    [SignByDate] DATETIME NULL, 
    [ExpiredDate] DATETIME NULL, 
    [SignedById] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [FK_EmployerAgreement_LegalEntity] FOREIGN KEY ([LegalEntityId]) REFERENCES [LegalEntity]([Id]), 
    CONSTRAINT [FK_EmployerAgreement_Template] FOREIGN KEY ([TemplateId]) REFERENCES [EmployerAgreementTemplate]([Id]), 
    CONSTRAINT [FK_EmployerAgreement_Status] FOREIGN KEY ([StatusId]) REFERENCES [EmployerAgreementStatus]([Id])
)
