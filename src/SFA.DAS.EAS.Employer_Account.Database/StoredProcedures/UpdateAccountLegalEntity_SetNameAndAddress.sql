CREATE PROCEDURE [employer_account].[UpdateAccountLegalEntity_SetNameAndAddress]
	@AccountId BIGINT,
	@LegalEntityId BIGINT,
	@Name nvarchar(100),
	@Address nvarchar(256)
AS

	UPDATE	employer_account.AccountLegalEntity
	SET		[Name] = @Name,
			Address = @Address
	WHERE	AccountId = @AccountId AND
			LegalEntityId = @LegalEntityId;

GO
