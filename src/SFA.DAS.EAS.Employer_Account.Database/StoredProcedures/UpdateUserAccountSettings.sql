CREATE PROCEDURE [employer_account].[UpdateUserAccountSettings]
(
	@UserRef UNIQUEIDENTIFIER,
	@NotificationSettings [employer_account].[UserNotificationSettingsTable] READONLY
)
AS
BEGIN

	UPDATE s
	set ReceiveNotifications = updated.[ReceiveNotifications]
	FROM [employer_account].[UserAccountSettings] s
	join [employer_account].[Account] a on a.Id = s.AccountId
	join [employer_account].[User] u on u.Id = s.UserId
	join @NotificationSettings updated on updated.AccountId = s.AccountId
	where
	u.UserRef = @UserRef

END