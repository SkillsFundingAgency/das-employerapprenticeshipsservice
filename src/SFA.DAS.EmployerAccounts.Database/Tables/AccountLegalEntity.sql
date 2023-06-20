CREATE TABLE [employer_account].[AccountLegalEntity]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(100) NOT NULL, 
    [Address] NVARCHAR(256) NULL, 
    [AccountId] BIGINT NOT NULL, 
    [LegalEntityId] BIGINT NOT NULL, 
    [Created] DATETIME NOT NULL, 
    [Modified] DATETIME NULL, 
    [SignedAgreementVersion] INT NULL, 
    [SignedAgreementId] BIGINT NULL, 
    [PendingAgreementVersion] INT NULL, 
    [PendingAgreementId] BIGINT NULL,
    [PublicHashedId] NVARCHAR(6) NULL, 
    [Deleted] DATETIME NULL, 
    CONSTRAINT [FK_AccountLegalEntity_Account] FOREIGN KEY ([AccountId]) REFERENCES [Employer_Account].[Account]([Id]),
    CONSTRAINT [FK_AccountLegalEntity_LegalEntity] FOREIGN KEY ([LegalEntityId]) REFERENCES [Employer_Account].[LegalEntity]([Id])
)

GO

CREATE INDEX [IX_AccountLegalEntity_AccountId] ON [employer_account].[AccountLegalEntity] ([AccountId]);
GO

CREATE INDEX [IX_AccountLegalEntity_LegalEntityId] ON [employer_account].[AccountLegalEntity] ([LegalEntityId]);
GO 

CREATE UNIQUE INDEX [IX_AccountLegalEntity_AccountIdLegalEntityId] ON [employer_account].[AccountLegalEntity] (AccountId, LegalEntityId) WHERE Deleted IS NULL;
GO

CREATE UNIQUE INDEX [IX_AccountLegalEntity_PublicHashedId] ON [employer_account].[AccountLegalEntity] ([PublicHashedId]) WHERE PublicHashedId IS NOT NULL AND Deleted IS NULL;
GO

