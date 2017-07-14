CREATE PROCEDURE [employer_account].[UpdateShowWizard]
	@hashedAccountId VARCHAR(20),
	@externalUserId UNIQUEIDENTIFIER,
	@showWizard BIT = 0
AS
	UPDATE mem
	SET [ShowWizard] = @showWizard
	FROM [employer_account].[Membership] mem
	INNER JOIN [employer_account].[User] u ON mem.UserId = u.Id
	INNER JOIN [employer_account].[Account] acc ON mem.AccountId = acc.Id
	WHERE acc.HashedId = @hashedAccountId AND u.UserRef = @externalUserId
