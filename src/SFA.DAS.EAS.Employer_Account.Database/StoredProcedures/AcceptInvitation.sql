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

	
	DECLARE @employerAgreementId BIGINT
	SELECT @employerAgreementId = a.Id
	FROM [employer_account].[EmployerAgreement] a
	JOIN [employer_account].[Membership] m on m.AccountId = a.AccountId
	WHERE a.AccountId = @accountId AND m.UserId = @userId

	INSERT INTO [employer_account].[UserLegalEntitySettings] (UserId, EmployerAgreementId, ReceiveNotifications)
	VALUES (@userId, @employerAgreementId, 1)

END
GO
