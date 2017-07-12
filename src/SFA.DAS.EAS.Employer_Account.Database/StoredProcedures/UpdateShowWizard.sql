CREATE PROCEDURE [employer_account].[UpdateShowWizard]
	@accountId BIGINT = 0,
	@userId BIGINT = 0,
	@showWizard BIT = 0
AS
	UPDATE [employer_account].[Membership] SET [ShowWizard] = @showWizard
	WHERE AccountId = @accountId AND UserId = @userId

