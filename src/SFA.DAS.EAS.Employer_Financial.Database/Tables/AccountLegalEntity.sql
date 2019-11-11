CREATE TABLE [employer_financial].[AccountLegalEntity]
(
	[Id] BIGINT NOT NULL PRIMARY KEY,
	[AccountId] BIGINT NOT NULL,
	[LegalEntityId] BIGINT NOT NULL,
	[SignedAgreementVersion] INT NULL,
	[SignedAgreementId] BIGINT NULL,
	[PendingAgreementId] BIGINT NULL
)
GO

CREATE INDEX [IX_AccountLegalEntity_AccountLegalEntityId] ON [employer_financial].[AccountLegalEntity] ([AccountId], [LegalEntityId])