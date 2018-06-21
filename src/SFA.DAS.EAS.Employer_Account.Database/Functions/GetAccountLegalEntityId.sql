CREATE FUNCTION [employer_Account].[GetAccountLegalEntityId]
(
	@accountId BIGINT,
	@legalEntityId BIGINT
)
RETURNS BIGINT
AS
BEGIN
	DECLARE @accountLegalEntityId AS BIGINT;

	SELECT	@accountLegalEntityId = Id
	FROM	employer_account.AccountLegalEntity 
	WHERE	AccountId = @accountId AND
			LegalEntityId = @legalEntityId;

	RETURN @accountLegalEntityId;
END
