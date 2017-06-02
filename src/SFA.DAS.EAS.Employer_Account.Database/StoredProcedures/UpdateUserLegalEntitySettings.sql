CREATE PROCEDURE [employer_account].[UpdateUserLegalEntitySettings]
(
	@UserRef UNIQUEIDENTIFIER,
	@AccountId BIGINT,
	@NotificationSettings [employer_account].[UserNotificationSettingsTable] READONLY
)
AS
BEGIN

	UPDATE s
	set ReceiveNotifications = updated.[ReceiveNotifications]
	FROM [employer_account].[UserLegalEntitySettings] s
	join [employer_account].[EmployerAgreement] a on a.Id = s.EmployerAgreementId
	join [employer_account].[LegalEntity] l on l.Id = a.LegalEntityId
	join [employer_account].[Membership] m on m.AccountId = a.AccountId and  m.UserId = s.UserId
	join [employer_account].[User] u on u.Id = s.UserId
	join @NotificationSettings updated on updated.[EmployerAgreementId] = s.EmployerAgreementId
	where
	u.UserRef = @UserRef
	and a.AccountId = @AccountId
	and a.StatusId <> 5

END