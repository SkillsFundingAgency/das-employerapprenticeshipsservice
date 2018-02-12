CREATE PROCEDURE [employer_account].[UpdateAccount_SetAccountHashes]
	@AccountId BIGINT,
	@ExternalHashedId VARCHAR(100),
	@HashedId VARCHAR(100)
AS
	UPDATE [employer_account].[Account]
	SET HashedId = @HashedId, ExternalHashedId =	
		CASE WHEN ExternalHashedId IS NULL 
			THEN @ExternalHashedId 
			ELSE ExternalHashedId 
		END
	WHERE Id = @AccountId 

