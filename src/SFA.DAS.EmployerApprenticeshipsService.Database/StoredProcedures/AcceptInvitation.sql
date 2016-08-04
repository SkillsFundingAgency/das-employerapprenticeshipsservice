CREATE PROCEDURE [dbo].[AcceptInvitation]
(
	@email NVARCHAR(100),
	@accountId BIGINT,
	@roleId TINYINT
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @userId BIGINT;

	SELECT @userId = Id
	FROM [dbo].[User]
	WHERE [Email] = @email;

	UPDATE [dbo].[Invitation]
	SET [Status] = 2
	WHERE [Email] = @email
		AND [AccountId] = @accountId;

	INSERT INTO [dbo].[Membership] ([AccountId], [UserId], [RoleId])
	VALUES (@accountId, @userId, @roleId);
END
GO
