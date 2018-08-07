CREATE PROCEDURE [employer_account].[UpdateAccountLegalEntity_SetNameAndAddress]
	@AccountLegalEntityId BIGINT,
	@Name nvarchar(100),
	@Address nvarchar(256)
AS

	UPDATE	employer_account.AccountLegalEntity
	SET		[Name] = @Name,
			Address = @Address
	WHERE	Id = @AccountLegalEntityId;

GO
