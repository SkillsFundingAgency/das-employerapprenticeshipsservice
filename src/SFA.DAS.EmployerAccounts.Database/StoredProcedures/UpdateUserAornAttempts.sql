CREATE PROCEDURE [employer_account].[UpdateUserAornAttempts]
(
	@UserRef UNIQUEIDENTIFIER,
	@Succeeded BIT
)
AS
BEGIN
	IF @Succeeded = 1
	BEGIN
		DELETE	uafa
		FROM	[employer_account].[UserAornFailedAttempts] uafa
		JOIN	[employer_account].[User] u on u.Id = uafa.UserId
		WHERE	u.UserRef = @UserRef
	END
	ELSE 
	BEGIN
		DECLARE @UserId BIGINT = (SELECT TOP(1) Id FROM [employer_account].[User] WHERE UserRef = @UserRef)

		IF @UserId IS NOT NULL
			INSERT INTO [employer_account].[UserAornFailedAttempts] (UserId, AttemptTimeStamp)
			VALUES (@UserId, GETDATE());
	END
END