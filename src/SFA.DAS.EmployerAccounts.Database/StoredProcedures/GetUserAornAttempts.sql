CREATE PROCEDURE [employer_account].[GetUserAornAttempts]
(
	@UserRef UNIQUEIDENTIFIER
)
AS
BEGIN
	SELECT	uafa.AttemptTimeStamp
	FROM	[employer_account].[User] u 
	JOIN	[employer_account].[UserAornFailedAttempts] uafa on u.Id = uafa.UserId
	WHERE	u.UserRef = @UserRef
END