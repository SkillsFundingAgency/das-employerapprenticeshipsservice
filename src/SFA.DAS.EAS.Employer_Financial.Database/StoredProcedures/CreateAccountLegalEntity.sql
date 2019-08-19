CREATE PROCEDURE [employer_financial].[CreateAccountLegalEntity]
	@id BIGINT,
	@accountId BIGINT,
	@legalEntityId BIGINT,
	@signedAgreementVersion INT,
	@signedAgreementId BIGINT,
	@pendingAgreementId BIGINT
AS
	INSERT INTO [employer_financial].[AccountLegalEntity]
		([Id],[AccountId],[LegalEntityId],[SignedAgreementVersion],[SignedAgreementId],[PendingAgreementId])
	VALUES
		(@id, @accountId, @legalEntityId, @signedAgreementVersion, @signedAgreementId, @pendingAgreementId)
