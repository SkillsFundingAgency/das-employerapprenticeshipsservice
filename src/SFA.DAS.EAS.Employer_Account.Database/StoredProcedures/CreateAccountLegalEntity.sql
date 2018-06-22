CREATE PROCEDURE [employer_account].[CreateAccountLegalEntity]
	@accountId bigint,
	@legalEntityId bigint,
	@employerName NVARCHAR(100), 
	@employerRegisteredAddress NVARCHAR(256),
	@accountLegalEntityId bigint output
AS
BEGIN

	SELECT	@accountLegalEntityId = Id
	FROM	employer_account.AccountLegalEntity 
	WHERE	AccountId = @accountId AND
			LegalEntityId = @legalEntityId;

	IF @accountLegalEntityId IS NULL
	BEGIN
		INSERT INTO [employer_account].[AccountLegalEntity](Name, Address, AccountId, LegalEntityId, Created) 
		VALUES (@employerName, @employerRegisteredAddress, @accountId, @legalEntityId, GetDate());

		SELECT @accountLegalEntityId = SCOPE_IDENTITY();
	END;

END
