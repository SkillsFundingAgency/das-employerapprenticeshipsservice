CREATE PROCEDURE [employer_account].[UpdateAccount_SetAccountExternalHashId]
	@AccountId BIGINT,
	@HashedId VARCHAR(100)
AS
	UPDATE [employer_account].[Account]
	SET ExternalHashedId =	CASE WHEN ExternalHashedId IS NULL THEN @HashedId 
								ELSE ExternalHashedId END
	WHERE Id = @AccountId 

