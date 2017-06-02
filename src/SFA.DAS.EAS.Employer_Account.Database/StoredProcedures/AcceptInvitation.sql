CREATE PROCEDURE [employer_account].[AcceptInvitation]
(
	@email NVARCHAR(255),
	@accountId BIGINT,
	@roleId TINYINT
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

	INSERT INTO [employer_account].[UserLegalEntitySettings] (UserId, EmployerAgreementId, ReceiveNotifications)
	SELECT @userId, a.Id, 1
	FROM [employer_account].[EmployerAgreement] a
	JOIN [employer_account].[Membership] m on m.AccountId = a.AccountId AND m.UserId = @userId
	WHERE
	a.AccountId = @accountId 
	AND m.UserId = @userId

END
GO
