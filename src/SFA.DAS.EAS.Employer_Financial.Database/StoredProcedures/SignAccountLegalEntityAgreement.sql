CREATE PROCEDURE [employer_financial].[SignAccountLegalEntityAgreement]
	@signedAgreementId BIGINT,
	@signedAgreementVersion INT,
	@accountId BIGINT,
	@legalEntityId BIGINT
AS
	UPDATE [employer_financial].[AccountLegalEntity]
	SET SignedAgreementId = @signedAgreementId, SignedAgreementVersion = @signedAgreementVersion
	WHERE AccountId = @accountId AND LegalEntityId = @legalEntityId
RETURN 0