CREATE PROCEDURE [account].[UpdateAccount_SetAccountHashId]
	@AccountId BIGINT,
	@HashedAccountId varchar(100)
AS
	UPDATE [account].[Account]
	SET HashedAccountId = @HashedAccountId
	WHERE Id = @AccountId 

