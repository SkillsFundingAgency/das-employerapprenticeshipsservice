CREATE PROCEDURE [employer_account].[GetUserAornAttempts]
(
	@UserRef UNIQUEIDENTIFIER
)
AS
BEGIN
	SELECT	u.UserRef,
			COUNT(*) AS NumberOfFailedAttempts,
			MAX(uafa.AttemptTimeStamp) AS LastAttemptTimeStamp
	FROM	[employer_account].[User] u 
	JOIN	[employer_account].[UserAornFailedAttempts] uafa on u.Id = uafa.UserId
	WHERE	u.UserRef = @UserRef
	GROUP BY u.UserRef
END