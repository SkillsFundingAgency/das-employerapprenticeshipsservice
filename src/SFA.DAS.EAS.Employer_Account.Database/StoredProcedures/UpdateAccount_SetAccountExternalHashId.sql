CREATE PROCEDURE [employer_account].[UpdateAccount_SetAccountExternalHashId]
	@AccountId BIGINT,
	@ExternalHashedId VARCHAR(100)
AS
	UPDATE [employer_account].[Account]
	SET ExternalHashedId =	CASE WHEN ExternalHashedId IS NULL THEN @ExternalHashedId 
								ELSE ExternalHashedId END
	WHERE Id = @AccountId 

