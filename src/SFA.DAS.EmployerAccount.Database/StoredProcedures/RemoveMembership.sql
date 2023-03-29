CREATE PROCEDURE [employer_account].[RemoveMembership]
(
	@UserId BIGINT,
	@AccountId BIGINT
)
AS
BEGIN
	DELETE
	FROM [employer_account].[Membership]
	WHERE AccountId = @AccountId
	AND UserId = @UserId

	DELETE
	FROM [employer_account].[UserAccountSettings]
	WHERE AccountId = @AccountId
	AND UserId = @UserId
END