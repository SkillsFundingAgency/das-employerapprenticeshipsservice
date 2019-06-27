CREATE PROCEDURE [employer_account].[UpdateAccount_SetAccountLevyType]
	@AccountId BIGINT,
	@LevyType TINYINT
AS
	UPDATE [employer_account].[Account]
	SET [LevyType] = @LevyType,
	[ModifiedDate] = GETDATE()
	WHERE Id = @AccountId 
