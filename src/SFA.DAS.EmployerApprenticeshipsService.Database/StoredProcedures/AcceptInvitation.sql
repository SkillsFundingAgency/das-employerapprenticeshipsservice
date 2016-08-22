CREATE PROCEDURE [account].[AcceptInvitation]
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
	FROM [account].[User]
	WHERE [Email] = @email;

	UPDATE [account].[Invitation]
	SET [Status] = 2
	WHERE [Email] = @email
		AND [AccountId] = @accountId;

	INSERT INTO [account].[Membership] ([AccountId], [UserId], [RoleId])
	VALUES (@accountId, @userId, @roleId);
END
GO
