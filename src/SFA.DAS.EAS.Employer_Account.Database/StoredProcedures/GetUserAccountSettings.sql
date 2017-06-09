CREATE PROCEDURE [employer_account].GetUserAccountSettings
(
	@UserRef UNIQUEIDENTIFIER
)
AS
BEGIN

	select
	s.Id,
	s.AccountId,
	a.HashedId as HashedAccountId,
	s.UserId,
	a.[Name],
	s.ReceiveNotifications
	from [employer_account].[UserAccountSettings] s
	join [employer_account].[Account] a on a.Id = s.AccountId
	join [employer_account].[User] u on u.Id = s.UserId
	where
	u.UserRef = @UserRef

END
