CREATE PROCEDURE [employer_account].GetUserAccountSettings
(
	@UserRef UNIQUEIDENTIFIER
)
AS
BEGIN

	select
	m.AccountId,
	a.HashedId as HashedAccountId,
	m.UserId,
	a.[Name],
	m.ReceiveNotifications
	from [employer_account].[Membership] m
	join [employer_account].[Account] a on a.Id = m.AccountId
	join [employer_account].[User] u on u.Id = m.UserId
	where
	u.UserRef = @UserRef

END
