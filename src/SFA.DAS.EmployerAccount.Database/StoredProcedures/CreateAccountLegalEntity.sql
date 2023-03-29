CREATE PROCEDURE [employer_account].[CreateAccountLegalEntity]
	@accountId bigint,
	@legalEntityId bigint,
	@employerName NVARCHAR(100), 
	@employerRegisteredAddress NVARCHAR(256),
	@accountLegalEntityId bigint output,
	@accountLegalEntityCreated BIT OUTPUT
AS
BEGIN

	SELECT	@accountLegalEntityId = Id
	FROM	employer_account.AccountLegalEntity 
	WHERE	AccountId = @accountId AND
			LegalEntityId = @legalEntityId AND
			Deleted IS NULL;

	SELECT @accountLegalEntityCreated = 0;

	IF @accountLegalEntityId IS NULL
	BEGIN
		INSERT INTO [employer_account].[AccountLegalEntity](Name, Address, AccountId, LegalEntityId, Created) 
		VALUES (@employerName, @employerRegisteredAddress, @accountId, @legalEntityId, GetUtcDate());

		SELECT	@accountLegalEntityId = SCOPE_IDENTITY(),
				@accountLegalEntityCreated = 1;

	END;
END
