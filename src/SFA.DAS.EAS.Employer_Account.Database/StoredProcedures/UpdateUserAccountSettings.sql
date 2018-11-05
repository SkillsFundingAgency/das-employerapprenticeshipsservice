CREATE PROCEDURE [employer_account].[UpdateUserAccountSettings]
(
	@UserRef UNIQUEIDENTIFIER,
	@NotificationSettings [employer_account].[UserNotificationSettingsTable] READONLY
)
AS
BEGIN

	UPDATE m
	set ReceiveNotifications = updated.[ReceiveNotifications]
	FROM [employer_account].[Membership] m
	join [employer_account].[Account] a on a.Id = m.AccountId
	join [employer_account].[User] u on u.Id = m.UserId
	join @NotificationSettings updated on updated.AccountId = m.AccountId
	where
	u.UserRef = @UserRef

END