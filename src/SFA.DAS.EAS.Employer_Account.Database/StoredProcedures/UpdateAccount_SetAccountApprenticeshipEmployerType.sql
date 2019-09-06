CREATE PROCEDURE [employer_account].[UpdateAccount_SetAccountApprenticeshipEmployerType]
	@AccountId BIGINT,
	@ApprenticeshipEmployerType TINYINT
AS
	UPDATE [employer_account].[Account]
	SET [ApprenticeshipEmployerType] = @ApprenticeshipEmployerType,
	[ModifiedDate] = GETDATE()
	WHERE Id = @AccountId 
