CREATE PROCEDURE [employer_account].[UpdateAccountLegalEntity_SetPublicHashedId]
	@AccountLegalEntityId BIGINT,
	@PublicHashedId nvarchar(6)
AS

	UPDATE	employer_account.AccountLegalEntity
	SET		[PublicHashedId] = @PublicHashedId
	WHERE	Id = @AccountLegalEntityId;

GO
