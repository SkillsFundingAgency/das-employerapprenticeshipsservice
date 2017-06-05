CREATE PROCEDURE [employer_account].[RemoveMembership]
(
	@UserId BIGINT,
	@AccountId BIGINT
)
AS
BEGIN

	DELETE FROM [employer_account].[Membership]
	WHERE AccountId = @AccountId AND UserId = @UserId;

	DELETE s
	FROM [employer_account].[UserAccountSettings] s
	where s.UserId = @UserId and s.Id = @AccountId

END
