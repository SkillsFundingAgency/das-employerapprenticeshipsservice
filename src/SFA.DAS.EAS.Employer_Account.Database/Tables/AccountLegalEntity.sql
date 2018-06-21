CREATE TABLE [employer_account].[AccountLegalEntity]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(100) NOT NULL, 
    [Address] NVARCHAR(256) NULL, 
    [CurrentSignedAgreement] BIGINT NULL, 
    [CurrentPendingAgreement] BIGINT NULL, 
    [AccountId] BIGINT NOT NULL, 
    [LegalEntityId] BIGINT NOT NULL, 
    [Created] DATETIME NOT NULL, 
    [Modified] DATETIME NULL, 
    CONSTRAINT [FK_AccountLegalEntity_Account] FOREIGN KEY ([AccountId]) REFERENCES [Employer_Account].[Account]([Id]),
    CONSTRAINT [FK_AccountLegalEntity_LegalEntity] FOREIGN KEY ([LegalEntityId]) REFERENCES [Employer_Account].[LegalEntity]([Id])
)

GO

CREATE INDEX [IX_AccountLegalEntity_AccountId] ON [employer_account].[AccountLegalEntity] ([AccountId])

GO

CREATE INDEX [IX_AccountLegalEntity_LegalEntityId] ON [employer_account].[AccountLegalEntity] ([LegalEntityId])
