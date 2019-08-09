CREATE TABLE [employer_financial].[AccountLegalEntity]
(
	[Id] BIGINT NOT NULL PRIMARY KEY,
	[AccountId] BIGINT NOT NULL,
	[LegalEntityId] BIGINT NOT NULL,
	[SignedAgreementVersion] INT NULL,
	[SignedAgreementId] BIGINT NULL,
	[PendingAgreementId] BIGINT NULL,
	[Deleted] DATETIME NULL
)
