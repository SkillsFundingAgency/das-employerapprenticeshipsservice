CREATE PROCEDURE [employer_account].[UpdateAccount_SetAccountLevyStatus]
	@AccountId BIGINT,
	@LevyStatus BIT
AS
	UPDATE [employer_account].[Account]
	SET [HasLevy] = @LevyStatus,
	[ModifiedDate] = GETDATE()
	WHERE Id = @AccountId 
