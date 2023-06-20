CREATE PROCEDURE [employer_account].[EnsureAccountLegalEntity]
	@accountId bigint,
	@legalEntityId bigint,
	@accountLegalEntityId bigint output
AS
BEGIN

	SELECT	@accountLegalEntityId = Id
	FROM	employer_account.AccountLegalEntity 
	WHERE	AccountId = @accountId AND
			LegalEntityId = @legalEntityId;

	IF @accountLegalEntityId IS NULL
	BEGIN
		INSERT INTO [employer_account].[AccountLegalEntity](Ref, AccessToken, RefreshToken, Name) 
		VALUES (@employerRef, @accessToken, @refreshToken, @employerRefName);
	END;

END
