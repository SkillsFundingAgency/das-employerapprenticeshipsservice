CREATE PROCEDURE [account].[UpdateAccount_SetAccountHashId]
	@AccountId BIGINT,
	@HashedId varchar(100)
AS
	UPDATE [account].[Account]
	SET HashedId = @HashedId
	WHERE Id = @AccountId 

