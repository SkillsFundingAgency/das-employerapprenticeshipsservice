CREATE PROCEDURE [employer_account].[AcceptInvitation]
(
	@email NVARCHAR(255),
	@accountId BIGINT,
	@roleId TINYINT,
	@membershipId BIGINT = null OUTPUT
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

	INSERT INTO [employer_account].[Membership] ([AccountId], [UserId], [RoleId])
	VALUES (@accountId, @userId, @roleId);

	SELECT @membershipId = SCOPE_IDENTITY()
END
GO
