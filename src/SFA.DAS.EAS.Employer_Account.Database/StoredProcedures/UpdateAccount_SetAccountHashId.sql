CREATE PROCEDURE [employer_account].[UpdateAccount_SetAccountHashId]
	@AccountId BIGINT,
	@HashedId varchar(100)
AS
	UPDATE [employer_account].[Account]
	SET HashedId = @HashedId
	WHERE Id = @AccountId 

