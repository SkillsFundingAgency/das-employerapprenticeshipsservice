CREATE PROCEDURE [employer_account].[UpdateAccount_SetAccountName]
	@AccountId BIGINT,
	@AccountName NVARCHAR(100)
AS
	UPDATE [employer_account].[Account]
	SET [Name] = @AccountName,
	[NameConfirmed] = 1,
	[ModifiedDate] = GETDATE()
	WHERE Id = @AccountId 
