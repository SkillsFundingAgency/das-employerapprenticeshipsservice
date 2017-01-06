CREATE PROCEDURE [account].[UpdateAccount_SetAccountName]
	@AccountId BIGINT,
	@AccountName NVARCHAR(100)
AS
	UPDATE [account].[Account]
	SET [Name] = @AccountName
	WHERE Id = @AccountId 
