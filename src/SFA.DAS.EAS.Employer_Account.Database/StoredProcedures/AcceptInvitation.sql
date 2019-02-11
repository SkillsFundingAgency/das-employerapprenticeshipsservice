CREATE PROCEDURE [employer_account].[AcceptInvitation]
(
	@email NVARCHAR(255),
	@accountId BIGINT,
	@Role TINYINT
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @userId BIGINT;

	SELECT @userId = Id
	FROM [employer_account].[User]
	WHERE [Email] = @email;

	UPDATE [employer_account].[Invitation]
	SET [Status] = 2
	WHERE [Email] = @email
		AND [AccountId] = @accountId;

	INSERT INTO [employer_account].[Membership] ([AccountId], [UserId], [Role])
	VALUES (@accountId, @userId, @Role);	

	INSERT INTO [employer_account].[UserAccountSettings] (UserId, AccountId, ReceiveNotifications)
	VALUES (@userId, @accountId, 1)

END
GO
